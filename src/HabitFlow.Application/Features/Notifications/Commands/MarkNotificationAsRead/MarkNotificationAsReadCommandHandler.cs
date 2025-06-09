using System;
using HabitFlow.Domain.Repositories;
using MediatR;

namespace HabitFlow.Application.Features.Notifications.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Unit>
{
    private readonly INotificationRepository _notificationRepository;

    public MarkNotificationAsReadCommandHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Unit> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.GetByIdAsync(request.NotificationId);

        if (notification == null || notification.UserId != request.UserId)
        {
            throw new ApplicationException("Notificação não encontrada ou não pertence ao usuário.");
        }

        notification.MarkAsRead();
        await _notificationRepository.UpdateAsync(notification);

        return Unit.Value;
    }
}
