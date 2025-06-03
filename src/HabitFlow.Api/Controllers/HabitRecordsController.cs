using System;
using System.Security.Claims;
using HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsDone;
using HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsDone.Dtos;
using HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsNotDone;
using HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsNotDone.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitFlow.Api.Controllers;

[ApiController]
[Route("habits/{habitId}/records")]
[Authorize]
public class HabitRecordsController : ControllerBase
{
    private readonly IMediator _mediator;

    public HabitRecordsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("done")]
    public async Task<IActionResult> MarkHabitAsDone(
        Guid habitId,
        [FromBody] MarkHabitAsDoneDto markHabitAsDoneDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var command = new MarkHabitAsDoneCommand(userId, habitId, markHabitAsDoneDto);
        var result = await _mediator.Send(command);

        return Ok(result);
    }
    [HttpPost("not-done")]
    public async Task<IActionResult> MarkHabitAsNotDone(
    Guid habitId,
    [FromBody] MarkHabitAsNotDoneDto markHabitAsNotDoneDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var command = new MarkHabitAsNotDoneCommand(userId, habitId, markHabitAsNotDoneDto);
        var result = await _mediator.Send(command);

        return Ok(result);
    }
}
