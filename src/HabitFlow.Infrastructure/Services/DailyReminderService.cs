using System;
using HabitFlow.Application.Common.Interfaces;
using HabitFlow.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HabitFlow.Infrastructure.Services;

public class DailyReminderService : BackgroundService
{
    private readonly ILogger<DailyReminderService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public DailyReminderService(
        ILogger<DailyReminderService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Daily Reminder Service running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRun = now.Date.AddDays(1).AddHours(9); // Próxima execução às 9h do dia seguinte

            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }

            var delay = nextRun - now;

            _logger.LogInformation("Next reminder check at {time}", nextRun);

            await Task.Delay(delay, stoppingToken);

            if (!stoppingToken.IsCancellationRequested)
            {
                await SendDailyRemindersAsync();
            }
        }
    }

    private async Task SendDailyRemindersAsync()
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var habitRepository = scope.ServiceProvider.GetRequiredService<IHabitRepository>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var habits = await habitRepository.GetHabitsAsync();

            foreach (var habit in habits)
            {
                var message = $"Lembrete: Não se esqueça de '{habit.Name}' hoje! Pouco a pouco se vai longe 😉";
                await notificationService.CreateReminderNotificationAsync(
                    habit.UserId,
                    habit.Id,
                    message);
            }

            _logger.LogInformation("Sent daily reminders for {count} habits", habits.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending daily reminders");
        }
    }
}
