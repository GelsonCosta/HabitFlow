using System;
using HabitFlow.Application.Common.Interfaces;
using HabitFlow.Application.Features.Users.Commands.RegisterUser.Dtos.RegisteredUserDto;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Repositories;
using MediatR;


namespace HabitFlow.Application.Features.Users.Commands.RegisterUser;


 public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisteredUserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<RegisteredUserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var userDto = request.UserDto;

            if (await _userRepository.EmailExistsAsync(userDto.Email))
            {
                throw new ApplicationException("Email já está em uso.");
            }

            var passwordHash = _passwordHasher.HashPassword(userDto.Password);
            var user = new User(userDto.Name, userDto.Email, passwordHash);

            await _userRepository.AddAsync(user);

            return new RegisteredUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                RegistrationDate = user.RegistrationDate
            };
        }
    }

