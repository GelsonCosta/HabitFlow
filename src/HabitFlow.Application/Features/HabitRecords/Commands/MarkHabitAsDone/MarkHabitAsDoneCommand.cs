using System;
using HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsDone.Dtos;
using MediatR;

namespace HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsDone;

public class MarkHabitAsDoneCommand : IRequest<HabitRecordDto>
{
    public Guid UserId { get; }
    public Guid HabitId { get; }
    public MarkHabitAsDoneDto RecordDto { get; }

    public MarkHabitAsDoneCommand(
        Guid userId,
        Guid habitId,
        MarkHabitAsDoneDto recordDto)
    {
        UserId = userId;
        HabitId = habitId;
        RecordDto = recordDto;
    }
}