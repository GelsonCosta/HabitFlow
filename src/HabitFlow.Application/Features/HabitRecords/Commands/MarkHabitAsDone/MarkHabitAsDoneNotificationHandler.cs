using System;
using HabitFlow.Application.Common.Interfaces;
using HabitFlow.Domain.Events;
using MediatR;

namespace HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsDone;

public class MarkHabitAsDoneNotificationHandler : INotificationHandler<HabitMarkedAsDoneEvent>
{
    private readonly INotificationService _notificationService;

    public MarkHabitAsDoneNotificationHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(HabitMarkedAsDoneEvent notification, CancellationToken cancellationToken)
    {
        // Exemplo: Enviar notificação de conquista se completou 7 dias seguidos
        if (notification.StreakLength == 7)
        {
            var message = $"Parabéns! Você completou o hábito '{notification.HabitName}' por 7 dias seguidos!";
            await _notificationService.CreateAchievementNotificationAsync(
                notification.UserId,
                message,
                notification.HabitId);
        }
    }
}
