using System;
using HabitFlow.Domain.Repositories;
using MediatR;

namespace HabitFlow.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;

public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, Unit>
{
    private readonly INotificationRepository _notificationRepository;

    public MarkAllNotificationsAsReadCommandHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Unit> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        await _notificationRepository.MarkAllAsReadAsync(request.UserId);
        return Unit.Value;
    }
}
