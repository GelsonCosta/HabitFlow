using System;
using HabitFlow.Domain.Enums;

namespace HabitFlow.Domain.Entities;

public class HabitRecord : BaseEntity
{
    public Guid HabitId { get; private set; }
    public DateTime Date { get; private set; }
    public HabitStatus Status { get; private set; }
    public string Note { get; private set; }
    public decimal? AchievedValue { get; private set; }

    // Construtor privado para EF Core
    private HabitRecord() { }

    public HabitRecord(
        Guid habitId,
        DateTime date,
        HabitStatus status,
        string note = null,
        decimal? achievedValue = null)
    {
        HabitId = habitId;
        Date = date;
        Status = status;
        Note = note;
        AchievedValue = achievedValue;
    }

    public void Update(
        HabitStatus status,
        string note = null,
        decimal? achievedValue = null)
    {
        Status = status;
        Note = note;
        AchievedValue = achievedValue;
    }
}

