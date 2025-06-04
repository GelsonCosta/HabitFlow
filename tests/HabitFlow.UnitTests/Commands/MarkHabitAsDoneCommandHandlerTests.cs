using System;
using AutoMapper;
using HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsDone;
using HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsDone.Dtos;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Enums;
using HabitFlow.Domain.Repositories;
using Moq;

namespace HabitFlow.UnitTests.Commands;

public class MarkHabitAsDoneCommandHandlerTests
{
    private readonly Mock<IHabitRepository> _habitRepositoryMock;
    private readonly Mock<IHabitRecordRepository> _habitRecordRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly MarkHabitAsDoneCommandHandler _handler;

    public MarkHabitAsDoneCommandHandlerTests()
    {
        _habitRepositoryMock = new Mock<IHabitRepository>();
        _habitRecordRepositoryMock = new Mock<IHabitRecordRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new MarkHabitAsDoneCommandHandler(
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

        var recordDto = new MarkHabitAsDoneDto
        {
            Date = DateTime.Today,
            Note = "Test note",
            AchievedValue = 1
        };

        var command = new MarkHabitAsDoneCommand(userId, habitId, recordDto);

        _habitRepositoryMock.Setup(x => x.GetByIdAsync(habitId))
            .ReturnsAsync(habit);

        _habitRecordRepositoryMock.Setup(x => x.GetByHabitAndDateAsync(habitId, recordDto.Date))
            .ReturnsAsync((HabitRecord)null);

        _mapperMock.Setup(x => x.Map<HabitRecordDto>(It.IsAny<HabitRecord>()))
            .Returns(new HabitRecordDto { Status = "Done" });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Done", result.Status);

        _habitRecordRepositoryMock.Verify(x => x.AddAsync(It.Is<HabitRecord>(r =>
            r.HabitId == habitId &&
            r.Status == HabitStatus.Done &&
            r.Note == recordDto.Note &&
            r.AchievedValue == recordDto.AchievedValue)), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_UpdateExistingRecord_When_RecordExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();
        var habit = new Habit(userId, "Test Habit", "daily", "1 time");
        var existingRecord = new HabitRecord(habitId, DateTime.Today, HabitStatus.NotDone);

        var recordDto = new MarkHabitAsDoneDto
        {
            Date = DateTime.Today,
            Note = "Updated note",
            AchievedValue = 1
        };

        var command = new MarkHabitAsDoneCommand(userId, habitId, recordDto);

        _habitRepositoryMock.Setup(x => x.GetByIdAsync(habitId))
            .ReturnsAsync(habit);

        _habitRecordRepositoryMock.Setup(x => x.GetByHabitAndDateAsync(habitId, recordDto.Date))
            .ReturnsAsync(existingRecord);

        _mapperMock.Setup(x => x.Map<HabitRecordDto>(It.IsAny<HabitRecord>()))
            .Returns(new HabitRecordDto { Status = "Done" });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Done", result.Status);

        _habitRecordRepositoryMock.Verify(x => x.UpdateAsync(It.Is<HabitRecord>(r =>
            r.Status == HabitStatus.Done &&
            r.Note == recordDto.Note &&
            r.AchievedValue == recordDto.AchievedValue)), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_HabitDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();
        var recordDto = new MarkHabitAsDoneDto { Date = DateTime.Today };

        var command = new MarkHabitAsDoneCommand(userId, habitId, recordDto);

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

        var recordDto = new MarkHabitAsDoneDto { Date = DateTime.Today };

        var command = new MarkHabitAsDoneCommand(userId, habitId, recordDto);

        _habitRepositoryMock.Setup(x => x.GetByIdAsync(habitId))
            .ReturnsAsync(habit);

        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
