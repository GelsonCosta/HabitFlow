using System;
using HabitFlow.Application.Common.Interfaces;
using HabitFlow.Application.Features.Users.Commands.LoginUser;
using HabitFlow.Application.Features.Users.Commands.LoginUser.Dtos;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Repositories;
using Moq;


namespace HabitFlow.UnitTests.Commands;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenServiceMock = new Mock<ITokenService>();
        _handler = new LoginUserCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _tokenServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnTokenAndUser_When_CredentialsAreValid()
    {
        // Arrange
        var user = new User("Test User", "test@email.com", "hashed_password");
        var loginDto = new LoginUserDto
        {
            Email = "test@email.com",
            Password = "correct_password"
        };

        var command = new LoginUserCommand(loginDto);

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(x => x.VerifyPassword(loginDto.Password, user.PasswordHash))
            .Returns(true);

        _tokenServiceMock.Setup(x => x.GenerateToken(user))
            .Returns("generated_jwt_token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("generated_jwt_token", result.Token);
        Assert.Equal(user.Id, result.User.Id);
        Assert.Equal(user.Name, result.User.Name);
        Assert.Equal(user.Email, result.User.Email);

        _userRepositoryMock.Verify(x => x.GetByEmailAsync(loginDto.Email), Times.Once);
        _passwordHasherMock.Verify(x => x.VerifyPassword(loginDto.Password, user.PasswordHash), Times.Once);
        _tokenServiceMock.Verify(x => x.GenerateToken(user), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_UserNotFound()
    {
        // Arrange
        var loginDto = new LoginUserDto
        {
            Email = "nonexistent@email.com",
            Password = "any_password"
        };

        var command = new LoginUserCommand(loginDto);

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _userRepositoryMock.Verify(x => x.GetByEmailAsync(loginDto.Email), Times.Once);
        _passwordHasherMock.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _tokenServiceMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_PasswordIsIncorrect()
    {
        // Arrange
        var user = new User("Test User", "test@email.com", "hashed_password");
        var loginDto = new LoginUserDto
        {
            Email = "test@email.com",
            Password = "wrong_password"
        };

        var command = new LoginUserCommand(loginDto);

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(x => x.VerifyPassword(loginDto.Password, user.PasswordHash))
            .Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _userRepositoryMock.Verify(x => x.GetByEmailAsync(loginDto.Email), Times.Once);
        _passwordHasherMock.Verify(x => x.VerifyPassword(loginDto.Password, user.PasswordHash ?? "password"), Times.Once);
        _tokenServiceMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }
}
