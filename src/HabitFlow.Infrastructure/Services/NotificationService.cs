using System;
using HabitFlow.Application.Common.Interfaces;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Enums;
using HabitFlow.Domain.Repositories;

namespace HabitFlow.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task CreateReminderNotificationAsync(Guid userId, Guid habitId, string message)
    {
        var notification = new Notification(
            userId,
            message,
            NotificationType.Reminder,
            habitId);

        await _notificationRepository.AddAsync(notification);
    }

    public async Task CreateAchievementNotificationAsync(Guid userId, string message, Guid? habitId = null)
    {
        var notification = new Notification(
            userId,
            message,
            NotificationType.Achievement,
            habitId);

        await _notificationRepository.AddAsync(notification);
    }
}
