using System;
using HabitFlow.Application.Features.Users.Commands.RegisterUser.Dtos;
using HabitFlow.Application.Features.Users.Commands.RegisterUser.Dtos.RegisteredUserDto;
using MediatR;

namespace HabitFlow.Application.Features.Users.Commands.RegisterUser;

    public class RegisterUserCommand : IRequest<RegisteredUserDto>
    {
        public RegisterUserDto UserDto { get; }

        public RegisterUserCommand(RegisterUserDto userDto)
        {
            UserDto = userDto;
        }
    }
