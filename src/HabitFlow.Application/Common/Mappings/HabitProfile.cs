using System;
using AutoMapper;
using HabitFlow.Application.Features.Habits.Queries.GetUserHabits;
using HabitFlow.Domain.Entities;

namespace HabitFlow.Application.Common.Mappings;

public class HabitProfile : Profile
{
    public HabitProfile()
    {
        CreateMap<Habit, HabitDto>();
    }
}
