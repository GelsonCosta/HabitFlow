using System;
using HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsDone.Dtos;
using HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsNotDone.Dtos;
using MediatR;

namespace HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsNotDone;

public class MarkHabitAsNotDoneCommand : IRequest<HabitRecordDto>
{
    public Guid UserId { get; }
    public Guid HabitId { get; }
    public MarkHabitAsNotDoneDto RecordDto { get; }

    public MarkHabitAsNotDoneCommand(
        Guid userId,
        Guid habitId,
        MarkHabitAsNotDoneDto recordDto)
    {
        UserId = userId;
        HabitId = habitId;
        RecordDto = recordDto;
    }
}
