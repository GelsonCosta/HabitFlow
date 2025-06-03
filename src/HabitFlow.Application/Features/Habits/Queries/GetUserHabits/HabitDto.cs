using System;

namespace HabitFlow.Application.Features.Habits.Queries.GetUserHabits;

public class HabitDto
{
    public Guid Id { get; set; }
    public Guid? CategoryId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Frequency { get; set; }
    public string Target { get; set; }
    public string Color { get; set; }
    public DateTime CreationDate { get; set; }
}
