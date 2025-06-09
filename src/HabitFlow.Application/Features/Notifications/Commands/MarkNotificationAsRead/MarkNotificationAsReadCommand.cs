using System;
using MediatR;

namespace HabitFlow.Application.Features.Notifications.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadCommand(Guid userId, Guid notificationId) : IRequest<Unit>
{
    public Guid UserId { get; } = userId;
    public Guid NotificationId { get; } = notificationId;
}
