using System;

namespace HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsDone.Dtos;

public class MarkHabitAsDoneDto
{
    public DateTime Date { get; set; }
    public string Note { get; set; }
    public decimal? AchievedValue { get; set; }
}