using System;
using AutoMapper;
using HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsDone.Dtos;
using HabitFlow.Domain.Entities;

namespace HabitFlow.Application.Common.Mappings;

public class HabitRecordProfile : Profile
{
    public HabitRecordProfile()
    {
        CreateMap<HabitRecord, HabitRecordDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
