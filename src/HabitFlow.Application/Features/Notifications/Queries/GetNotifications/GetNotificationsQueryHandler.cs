using System;
using AutoMapper;
using HabitFlow.Application.Features.Notifications.Queries.GetNotifications.Dtos;
using HabitFlow.Domain.Repositories;
using MediatR;

namespace HabitFlow.Application.Features.Notifications.Queries.GetNotifications;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, IEnumerable<NotificationDto>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;

    public GetNotificationsQueryHandler(
        INotificationRepository notificationRepository,
        IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _notificationRepository.GetByUserIdAsync(request.UserId, request.IsRead);
        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }
}
