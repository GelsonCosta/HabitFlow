using System;
using HabitFlow.Application.Features.Habits.Commands;
using HabitFlow.Application.Features.Habits.Commands.CreateHabit.Dtos;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Repositories;
using Moq;

namespace HabitFlow.UnitTests.Commands;

public class CreateHabitCommandHandlerTests
{
    private readonly Mock<IHabitRepository> _habitRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly CreateHabitCommandHandler _handler;

    public CreateHabitCommandHandlerTests()
    {
        _habitRepositoryMock = new Mock<IHabitRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _handler = new CreateHabitCommandHandler(
            _habitRepositoryMock.Object,
            _userRepositoryMock.Object,
            _categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnCreatedHabitDto_When_InputIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var habitDto = new CreateHabitDto
        {
            Name = "Beber Água",
            Description = "Beber 2 litros de água por dia",
            Frequency = "daily",
            Target = "2 liters",
            Color = "#4A90E2"
        };

        var command = new CreateHabitCommand(userId, habitDto);

        _userRepositoryMock.Setup(x => x.ExistsAsync(userId))
            .ReturnsAsync(true);

        _habitRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Habit>()))
            .Returns(Task.CompletedTask)
            .Callback<Habit>(h => h.Id = Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(habitDto.Name, result.Name);
        Assert.Equal(habitDto.Description, result.Description);
        Assert.Equal(habitDto.Frequency, result.Frequency);
        Assert.Equal(habitDto.Target, result.Target);
        Assert.Equal(habitDto.Color, result.Color);

        _userRepositoryMock.Verify(x => x.ExistsAsync(userId), Times.Once);
        _habitRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Habit>()), Times.Once);
        _categoryRepositoryMock.Verify(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnCreatedHabitDto_When_CategoryIsProvided()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var habitDto = new CreateHabitDto
        {
            Name = "Beber Água",
            Frequency = "daily",
            Target = "2 liters",
            CategoryId = categoryId
        };

        var command = new CreateHabitCommand(userId, habitDto);

        _userRepositoryMock.Setup(x => x.ExistsAsync(userId))
            .ReturnsAsync(true);

        _categoryRepositoryMock.Setup(x => x.ExistsAsync(categoryId, userId))
            .ReturnsAsync(true);

        _habitRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Habit>()))
            .Returns(Task.CompletedTask)
            .Callback<Habit>(h => h.Id = Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryId, result.CategoryId);

        _categoryRepositoryMock.Verify(x => x.ExistsAsync(categoryId, userId), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_UserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var habitDto = new CreateHabitDto
        {
            Name = "Beber Água",
            Frequency = "daily",
            Target = "2 liters"
        };

        var command = new CreateHabitCommand(userId, habitDto);

        _userRepositoryMock.Setup(x => x.ExistsAsync(userId))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_CategoryDoesNotBelongToUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var habitDto = new CreateHabitDto
        {
            Name = "Beber Água",
            Frequency = "daily",
            Target = "2 liters",
            CategoryId = categoryId
        };

        var command = new CreateHabitCommand(userId, habitDto);

        _userRepositoryMock.Setup(x => x.ExistsAsync(userId))
            .ReturnsAsync(true);

        _categoryRepositoryMock.Setup(x => x.ExistsAsync(categoryId, userId))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
