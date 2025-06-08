using AutoMapper;
using HabitFlow.Application.Features.Habits.Commands.UpdateHabit;
using HabitFlow.Application.Features.Habits.Commands.UpdateHabit.Dtos;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Repositories;
using Moq;


namespace HabitFlow.UnitTests.Commands;

public class UpdateHabitCommandHandlerTests
{
    private readonly Mock<IHabitRepository> _habitRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateHabitCommandHandler _handler;

    public UpdateHabitCommandHandlerTests()
    {
        _habitRepositoryMock = new Mock<IHabitRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateHabitCommandHandler(
            _habitRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnUpdatedHabit_When_InputIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();
        var habit = new Habit(userId, "Old Name", "Old Freq", "Old Target");

        var updateDto = new UpdateHabitDto
        {
            Name = "New Name",
            Description = "New Desc",
            Frequency = "New Freq",
            Target = "New Target",
            Color = "#FFFFFF",
            CategoryId = Guid.NewGuid()
        };

        var command = new UpdateHabitCommand(userId, habitId, updateDto);

        _habitRepositoryMock.Setup(x => x.GetByIdAsync(habitId))
            .ReturnsAsync(habit);

        _categoryRepositoryMock.Setup(x => x.ExistsAsync(updateDto.CategoryId.Value, userId))
            .ReturnsAsync(true);

        _mapperMock.Setup(x => x.Map<UpdatedHabitDto>(It.IsAny<Habit>()))
            .Returns(new UpdatedHabitDto
            {
                Name = updateDto.Name,
                Description = updateDto.Description,
                Frequency = updateDto.Frequency,
                Target = updateDto.Target,
                Color = updateDto.Color,
                CategoryId = updateDto.CategoryId
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateDto.Name, result.Name);
        Assert.Equal(updateDto.Description, result.Description);
        Assert.Equal(updateDto.Frequency, result.Frequency);
        Assert.Equal(updateDto.Target, result.Target);
        Assert.Equal(updateDto.Color, result.Color);
        Assert.Equal(updateDto.CategoryId, result.CategoryId);

        _habitRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Habit>(h =>
            h.Name == updateDto.Name &&
            h.Description == updateDto.Description &&
            h.Frequency == updateDto.Frequency &&
            h.Target == updateDto.Target &&
            h.Color == updateDto.Color &&
            h.CategoryId == updateDto.CategoryId)), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_HabitDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();
        var updateDto = new UpdateHabitDto { Name = "Test" };

        var command = new UpdateHabitCommand(userId, habitId, updateDto);

        _habitRepositoryMock.Setup(x => x.GetByIdAsync(habitId))
            .ReturnsAsync((Habit)null);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_CategoryDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();
        var habit = new Habit(userId, "Test", "Freq", "Target");
        var categoryId = Guid.NewGuid();

        var updateDto = new UpdateHabitDto
        {
            Name = "Test",
            CategoryId = categoryId
        };

        var command = new UpdateHabitCommand(userId, habitId, updateDto);

        _habitRepositoryMock.Setup(x => x.GetByIdAsync(habitId))
            .ReturnsAsync(habit);

        _categoryRepositoryMock.Setup(x => x.ExistsAsync(categoryId, userId))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
