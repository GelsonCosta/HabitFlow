using HabitFlow.Application.Features.Users.Commands.LoginUser;
using HabitFlow.Application.Features.Users.Commands.LoginUser.Dtos;
using HabitFlow.Application.Features.Users.Commands.RegisterUser;
using HabitFlow.Application.Features.Users.Commands.RegisterUser.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HabitFlow.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            var command = new RegisterUserCommand(registerUserDto);
            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(Register), result);
        }
        [HttpPost("regi")]
        public async Task<IActionResult> Regi()
        {
            return Ok("test");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            var command = new LoginUserCommand(loginUserDto);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
