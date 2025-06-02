using System;

namespace HabitFlow.Application.Features.Users.Commands.LoginUser.Dtos;

public class LoginUserDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
public class LoggedInUserDto
{
    public string Token { get; set; }
    public UserDto User { get; set; }
}
public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}