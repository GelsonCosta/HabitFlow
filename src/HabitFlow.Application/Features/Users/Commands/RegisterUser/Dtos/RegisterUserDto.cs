using System;

namespace HabitFlow.Application.Features.Users.Commands.RegisterUser.Dtos;

public class RegisterUserDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
