using System;

namespace HabitFlow.Application.Features.Users.Commands.RegisterUser.Dtos.RegisteredUserDto;

public class RegisteredUserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime RegistrationDate { get; set; }
}
