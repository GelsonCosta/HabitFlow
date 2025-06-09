using System;
using MediatR;

namespace HabitFlow.Domain.Events;

public class HabitMarkedAsDoneEvent : INotification
{
    public Guid UserId { get; }
    public Guid HabitId { get; }
    public string HabitName { get; }
    public int StreakLength { get; }

    public HabitMarkedAsDoneEvent(Guid userId, Guid habitId, string habitName, int streakLength)
    {
        UserId = userId;
        HabitId = habitId;
        HabitName = habitName;
        StreakLength = streakLength;
    }
}
