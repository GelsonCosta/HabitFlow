using System;
using HabitFlow.Application.Features.Habits.Commands.CreateHabit.Dtos;
using MediatR;

namespace HabitFlow.Application.Features.Habits.Commands;


public class CreateHabitCommand : IRequest<CreatedHabitDto>
{
    public Guid UserId { get; }
    public CreateHabitDto HabitDto { get; }

    public CreateHabitCommand(Guid userId, CreateHabitDto habitDto)
    {
        UserId = userId;
        HabitDto = habitDto;
    }
}
