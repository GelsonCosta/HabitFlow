using System;

namespace HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsNotDone.Dtos;

public class MarkHabitAsNotDoneDto
{
    public DateTime Date { get; set; }
    public string Note { get; set; }
}
