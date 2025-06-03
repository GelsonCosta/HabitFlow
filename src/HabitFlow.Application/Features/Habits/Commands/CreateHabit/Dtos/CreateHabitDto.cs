using System;

namespace HabitFlow.Application.Features.Habits.Commands.CreateHabit.Dtos;

public class CreateHabitDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid? CategoryId { get; set; }
    public string Frequency { get; set; }
    public string Target { get; set; }
    public string Color { get; set; }
}
