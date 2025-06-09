using System;
using HabitFlow.Domain.Entities;

namespace HabitFlow.Domain.Repositories;

public interface INotificationRepository
{
    Task<Notification> GetByIdAsync(Guid id);
    Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, bool? isRead = null);
    Task AddAsync(Notification notification);
    Task UpdateAsync(Notification notification);
    Task MarkAllAsReadAsync(Guid userId);
}
