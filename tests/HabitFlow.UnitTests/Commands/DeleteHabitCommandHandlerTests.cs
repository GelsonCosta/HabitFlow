using AutoMapper;
using HabitFlow.Application.Features.Habits.Commands.DeleteHabit;
using HabitFlow.Application.Features.Habits.Commands.UpdateHabit;
using HabitFlow.Application.Features.Habits.Commands.UpdateHabit.Dtos;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Enums;
using HabitFlow.Domain.Repositories;
using MediatR;
using Moq;


namespace HabitFlow.UnitTests.Commands;

public class DeleteHabitCommandHandlerTests
{
    private readonly Mock<IHabitRepository> _habitRepositoryMock;
    private readonly Mock<IHabitRecordRepository> _habitRecordRepositoryMock;
    private readonly DeleteHabitCommandHandler _handler;

    public DeleteHabitCommandHandlerTests()
    {
        _habitRepositoryMock = new Mock<IHabitRepository>();
        _habitRecordRepositoryMock = new Mock<IHabitRecordRepository>();
        _handler = new DeleteHabitCommandHandler(
            _habitRepositoryMock.Object,
            _habitRecordRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_DeleteHabitAndRecords_When_InputIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();
        var habit = new Habit(userId, "Test", "Freq", "Target");
        var records = new List<HabitRecord>
            {
                new HabitRecord(habitId, DateTime.Today, HabitStatus.Done),
                new HabitRecord(habitId, DateTime.Today.AddDays(-1), HabitStatus.NotDone)
            };

        var command = new DeleteHabitCommand(userId, habitId);

        _habitRepositoryMock.Setup(x => x.GetByIdAsync(habitId))
            .ReturnsAsync(habit);

        _habitRecordRepositoryMock.Setup(x => x.GetByHabitIdAsync(habitId, null, null))
            .ReturnsAsync(records);


        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
        _habitRecordRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<HabitRecord>()), Times.Exactly(2));
        _habitRepositoryMock.Verify(x => x.DeleteAsync(habit), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_HabitDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();

        var command = new DeleteHabitCommand(userId, habitId);

        _habitRepositoryMock.Setup(x => x.GetByIdAsync(habitId))
            .ReturnsAsync((Habit)null);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _habitRecordRepositoryMock.Verify(x => x.GetByHabitIdAsync(It.IsAny<Guid>(),null,null), Times.Never);
        _habitRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Habit>()), Times.Never);
    }
}

