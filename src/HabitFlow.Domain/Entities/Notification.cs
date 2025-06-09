using System;
using HabitFlow.Domain.Enums;

namespace HabitFlow.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid? HabitId { get; private set; }
    public string Message { get; private set; }
    public NotificationType Type { get; private set; }
    public DateTime SendDate { get; private set; }
    public bool IsRead { get; private set; }

    // Construtor privado para EF Core
    private Notification() { }

    public Notification(
        Guid userId,
        string message,
        NotificationType type,
        Guid? habitId = null)
    {
        UserId = userId;
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Type = type;
        HabitId = habitId;
        SendDate = DateTime.UtcNow;
        IsRead = false;
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}
