using System.Security.Claims;
using HabitFlow.Application.Features.Habits.Commands;
using HabitFlow.Application.Features.Habits.Commands.CreateHabit.Dtos;
using HabitFlow.Application.Features.Habits.Queries.GetUserHabits;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HabitFlow.Api.Controllers
{
    [ApiController]
    [Route("habits")]
    [Authorize]
    public class HabitsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HabitsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateHabit([FromBody] CreateHabitDto createHabitDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var command = new CreateHabitCommand(userId, createHabitDto);
            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(CreateHabit), result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserHabits()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var query = new GetUserHabitsQuery(userId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
