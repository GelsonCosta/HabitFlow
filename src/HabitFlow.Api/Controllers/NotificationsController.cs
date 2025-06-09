using System.Security.Claims;
using HabitFlow.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;
using HabitFlow.Application.Features.Notifications.Commands.MarkNotificationAsRead;
using HabitFlow.Application.Features.Notifications.Queries.GetNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HabitFlow.Api.Controllers
{
    [ApiController]
    [Route("notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] bool? isRead)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var query = new GetNotificationsQuery(userId, isRead);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkNotificationAsRead(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var command = new MarkNotificationAsReadCommand(userId, id);
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var command = new MarkAllNotificationsAsReadCommand(userId);
            await _mediator.Send(command);

            return NoContent();
        }
    }
}
