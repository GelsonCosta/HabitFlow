using System;
using HabitFlow.Application.Common.Interfaces;
using HabitFlow.Application.Features.Users.Commands.RegisterUser;
using HabitFlow.Application.Features.Users.Commands.RegisterUser.Dtos;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Repositories;
using Moq;

namespace HabitFlow.UnitTests.Commands;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _handler = new RegisterUserCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnRegisteredUserDto_When_InputIsValid()
    {
        // Arrange
        var userDto = new RegisterUserDto
        {
            Name = "Test User",
            Email = "test@email.com",
            Password = "Test@123"
        };

        var command = new RegisterUserCommand(userDto);

        _userRepositoryMock.Setup(x => x.EmailExistsAsync(userDto.Email))
            .ReturnsAsync(false);

        _passwordHasherMock.Setup(x => x.HashPassword(userDto.Password))
            .Returns("hashed_password");

        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask)
            .Callback<User>(u => u.Id = Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userDto.Name, result.Name);
        Assert.Equal(userDto.Email, result.Email);
        Assert.NotEqual(default(DateTime), result.RegistrationDate);

        _userRepositoryMock.Verify(x => x.EmailExistsAsync(userDto.Email), Times.Once);
        _passwordHasherMock.Verify(x => x.HashPassword(userDto.Password), Times.Once);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_EmailAlreadyExists()
    {
        // Arrange
        var userDto = new RegisterUserDto
        {
            Name = "Test User",
            Email = "existing@email.com",
            Password = "Test@123"
        };

        var command = new RegisterUserCommand(userDto);

        _userRepositoryMock.Setup(x => x.EmailExistsAsync(userDto.Email))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _userRepositoryMock.Verify(x => x.EmailExistsAsync(userDto.Email), Times.Once);
        _passwordHasherMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Theory]
    [InlineData(null, "test@email.com", "Test@123")]
    [InlineData("Test User", null, "Test@123")]
    [InlineData("Test User", "test@email.com", null)]
    public async Task Handle_Should_ThrowException_When_InputIsInvalid(string name, string email, string password)
    {
        // Arrange
        var userDto = new RegisterUserDto
        {
            Name = name,
            Email = email,
            Password = password
        };

        var command = new RegisterUserCommand(userDto);

        _userRepositoryMock.Setup(x => x.EmailExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
