using System;
using AutoMapper;
using HabitFlow.Application.Features.Notifications.Queries.GetNotifications.Dtos;
using HabitFlow.Domain.Entities;

namespace HabitFlow.Application.Common.Mappings;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
    }
}
