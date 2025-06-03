using System;

namespace HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsDone.Dtos;

public class HabitRecordDto
{
    public Guid Id { get; set; }
    public Guid HabitId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; }
    public string Note { get; set; }
    public decimal? AchievedValue { get; set; }
}
