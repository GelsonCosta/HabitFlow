using System;
using AutoMapper;
using HabitFlow.Application.Features.Habits.Queries.GetUserHabits;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Repositories;
using Moq;

namespace HabitFlow.UnitTests.Queries;

public class GetUserHabitsQueryHandlerTests
{
    private readonly Mock<IHabitRepository> _habitRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetUserHabitsQueryHandler _handler;

    public GetUserHabitsQueryHandlerTests()
    {
        _habitRepositoryMock = new Mock<IHabitRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetUserHabitsQueryHandler(
            _habitRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnHabitDtos_When_HabitsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var habits = new List<Habit>
            {
                new Habit(userId, "Habit 1", "daily", "1 time"),
                new Habit(userId, "Habit 2", "weekly", "3 times")
            };

        var habitDtos = new List<HabitDto>
            {
                new HabitDto { Name = "Habit 1" },
                new HabitDto { Name = "Habit 2" }
            };

        var query = new GetUserHabitsQuery(userId);

        _habitRepositoryMock.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(habits);

        _mapperMock.Setup(x => x.Map<IEnumerable<HabitDto>>(habits))
            .Returns(habitDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());

        _habitRepositoryMock.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
        _mapperMock.Verify(x => x.Map<IEnumerable<HabitDto>>(habits), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_When_NoHabitsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserHabitsQuery(userId);

        _habitRepositoryMock.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<Habit>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
