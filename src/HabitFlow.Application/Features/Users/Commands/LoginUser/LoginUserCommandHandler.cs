using System;
using HabitFlow.Application.Common.Interfaces;
using HabitFlow.Application.Features.Users.Commands.LoginUser.Dtos;
using HabitFlow.Domain.Repositories;
using MediatR;

namespace HabitFlow.Application.Features.Users.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoggedInUserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoggedInUserDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var loginDto = request.LoginDto;

        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null)
        {
            throw new ApplicationException("Credenciais inválidas.");
        }

        if (!VerifyPassword(loginDto.Password, _passwordHasher))
        {
            throw new ApplicationException("Credenciais inválidas.");
        }

        var token = _tokenService.GenerateToken(user);

        return new LoggedInUserDto
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            }
        };
    }
    public bool VerifyPassword(string password, IPasswordHasher passwordHasher)
    {
        return passwordHasher.VerifyPassword(password,passwordHasher.HashPassword(password));
    }
}
