using System;
using MediatR;

namespace HabitFlow.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;

public class MarkAllNotificationsAsReadCommand : IRequest<Unit>
{
    public Guid UserId { get; }

    public MarkAllNotificationsAsReadCommand(Guid userId)
    {
        UserId = userId;
    }
}
