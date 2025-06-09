using System;
using HabitFlow.Application.Features.Notifications.Queries.GetNotifications.Dtos;
using MediatR;

namespace HabitFlow.Application.Features.Notifications.Queries.GetNotifications;

public class GetNotificationsQuery : IRequest<IEnumerable<NotificationDto>>
{
    public Guid UserId { get; }
    public bool? IsRead { get; }

    public GetNotificationsQuery(Guid userId, bool? isRead = null)
    {
        UserId = userId;
        IsRead = isRead;
    }
}
