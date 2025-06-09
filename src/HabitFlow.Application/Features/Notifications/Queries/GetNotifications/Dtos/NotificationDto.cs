using System;

namespace HabitFlow.Application.Features.Notifications.Queries.GetNotifications.Dtos;

public class NotificationDto
{
    public Guid Id { get; set; }
    public Guid? HabitId { get; set; }
    public string Message { get; set; }
    public string Type { get; set; }
    public DateTime SendDate { get; set; }
    public bool IsRead { get; set; }
}
