using AutoMapper;
using HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsDone.Dtos;
using HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsNotDone;
using HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsNotDone.Dtos;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Enums;
using HabitFlow.Domain.Repositories;
using Moq;

namespace HabitFlow.UnitTests.Commands;

public class MarkHabitAsNotDoneCommandHandlerTests
{
    private readonly Mock<IHabitRepository> _habitRepositoryMock;
    private readonly Mock<IHabitRecordRepository> _habitRecordRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly MarkHabitAsNotDoneCommandHandler _handler;

    public MarkHabitAsNotDoneCommandHandlerTests()
    {
        _habitRepositoryMock = new Mock<IHabitRepository>();
        _habitRecordRepositoryMock = new Mock<IHabitRecordRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new MarkHabitAsNotDoneCommandHandler(
            _habitRepositoryMock.Object,
            _habitRecordRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_Should_CreateNewRecord_When_NoRecordExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();
        var habit = new Habit(userId, "Test Habit", "daily", "1 time");

        var recordDto = new MarkHabitAsNotDoneDto
        {
            Date = DateTime.Today,
            Note = "Não consegui hoje"
        };

        var command = new MarkHabitAsNotDoneCommand(userId, habitId, recordDto);

        _habitRepositoryMock.Setup(x => x.GetByIdAsync(habitId))
            .ReturnsAsync(habit);

        _habitRecordRepositoryMock.Setup(x => x.GetByHabitAndDateAsync(habitId, recordDto.Date))
            .ReturnsAsync((HabitRecord)null);

        _mapperMock.Setup(x => x.Map<HabitRecordDto>(It.IsAny<HabitRecord>()))
            .Returns(new HabitRecordDto { Status = "NotDone" });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("NotDone", result.Status);

        _habitRecordRepositoryMock.Verify(x => x.AddAsync(It.Is<HabitRecord>(r =>
            r.HabitId == habitId &&
            r.Status == HabitStatus.NotDone &&
            r.Note == recordDto.Note &&
            r.AchievedValue == null)), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_UpdateExistingRecord_When_RecordExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();
        var habit = new Habit(userId, "Test Habit", "daily", "1 time");
        var existingRecord = new HabitRecord(habitId, DateTime.Today, HabitStatus.Done);

        var recordDto = new MarkHabitAsNotDoneDto
        {
            Date = DateTime.Today,
            Note = "Não consegui hoje"
        };

        var command = new MarkHabitAsNotDoneCommand(userId, habitId, recordDto);

        _habitRepositoryMock.Setup(x => x.GetByIdAsync(habitId))
            .ReturnsAsync(habit);

        _habitRecordRepositoryMock.Setup(x => x.GetByHabitAndDateAsync(habitId, recordDto.Date))
            .ReturnsAsync(existingRecord);

        _mapperMock.Setup(x => x.Map<HabitRecordDto>(It.IsAny<HabitRecord>()))
            .Returns(new HabitRecordDto { Status = "NotDone" });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("NotDone", result.Status);

        _habitRecordRepositoryMock.Verify(x => x.UpdateAsync(It.Is<HabitRecord>(r =>
            r.Status == HabitStatus.NotDone &&
            r.Note == recordDto.Note &&
            r.AchievedValue == null)), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_HabitDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();
        var recordDto = new MarkHabitAsNotDoneDto { Date = DateTime.Today };

        var command = new MarkHabitAsNotDoneCommand(userId, habitId, recordDto);

        _habitRepositoryMock.Setup(x => x.GetByIdAsync(habitId))
            .ReturnsAsync((Habit)null);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_HabitDoesNotBelongToUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var habitId = Guid.NewGuid();
        var habit = new Habit(otherUserId, "Test Habit", "daily", "1 time");

        var recordDto = new MarkHabitAsNotDoneDto { Date = DateTime.Today };

        var command = new MarkHabitAsNotDoneCommand(userId, habitId, recordDto);

        _habitRepositoryMock.Setup(x => x.GetByIdAsync(habitId))
            .ReturnsAsync(habit);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
