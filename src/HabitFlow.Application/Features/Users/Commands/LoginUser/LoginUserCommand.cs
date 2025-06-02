using System;
using HabitFlow.Application.Features.Users.Commands.LoginUser.Dtos;
using MediatR;

namespace HabitFlow.Application.Features.Users.Commands.LoginUser;

public class LoginUserCommand(LoginUserDto loginDto) : IRequest<LoggedInUserDto>
{
    public LoginUserDto LoginDto { get; } = loginDto;
}