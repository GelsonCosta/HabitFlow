using System;
using HabitFlow.Application.Features.Habits.Commands.UpdateHabit.Dtos;
using MediatR;

namespace HabitFlow.Application.Features.Habits.Commands.UpdateHabit;

public class UpdateHabitCommand : IRequest<UpdatedHabitDto>
{
    public Guid UserId { get; }
    public Guid HabitId { get; }
    public UpdateHabitDto HabitDto { get; }

    public UpdateHabitCommand(Guid userId, Guid habitId, UpdateHabitDto habitDto)
    {
        UserId = userId;
        HabitId = habitId;
        HabitDto = habitDto;
    }
}