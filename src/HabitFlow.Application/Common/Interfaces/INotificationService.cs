using System;

namespace HabitFlow.Application.Common.Interfaces;

public interface INotificationService
{
    Task CreateReminderNotificationAsync(Guid userId, Guid habitId, string message);
    Task CreateAchievementNotificationAsync(Guid userId, string message, Guid? habitId = null);
}
