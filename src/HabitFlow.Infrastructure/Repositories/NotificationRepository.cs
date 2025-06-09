using System;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Repositories;
using HabitFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HabitFlow.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Notification> GetByIdAsync(Guid id)
    {
        return await _context.Notifications.FindAsync(id);
    }

public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, bool? isRead = null)
{
    IQueryable<Notification> query = _context.Notifications
        .Where(n => n.UserId == userId)
        .OrderByDescending(n => n.SendDate);

    if (isRead.HasValue)
    {
        query = query.Where(n => n.IsRead == isRead.Value);
    }

    return await query.ToListAsync();
}

    public async Task AddAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Notification notification)
    {
        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var unreadNotifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
        }

        await _context.SaveChangesAsync();
    }
}
