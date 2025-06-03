using System;
using MediatR;

namespace HabitFlow.Application.Features.Habits.Queries.GetUserHabits;

public class GetUserHabitsQuery(Guid userId) : IRequest<IEnumerable<HabitDto>>
{
    public Guid UserId { get; } = userId;
}