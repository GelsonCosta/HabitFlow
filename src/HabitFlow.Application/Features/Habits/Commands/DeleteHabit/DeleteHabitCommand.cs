using System;
using MediatR;

namespace HabitFlow.Application.Features.Habits.Commands.DeleteHabit;

public class DeleteHabitCommand : IRequest<Unit>
{
    public Guid UserId { get; }
    public Guid HabitId { get; }

    public DeleteHabitCommand(Guid userId, Guid habitId)
    {
        UserId = userId;
        HabitId = habitId;
    }
}
