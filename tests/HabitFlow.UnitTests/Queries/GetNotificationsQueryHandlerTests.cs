using System;
using AutoMapper;
using HabitFlow.Application.Features.Notifications.Queries.GetNotifications;
using HabitFlow.Application.Features.Notifications.Queries.GetNotifications.Dtos;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Enums;
using HabitFlow.Domain.Repositories;
using Moq;

namespace HabitFlow.UnitTests.Queries;

public class GetNotificationsQueryHandlerTests
{
    private readonly Mock<INotificationRepository> _notificationRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetNotificationsQueryHandler _handler;

    public GetNotificationsQueryHandlerTests()
    {
        _notificationRepositoryMock = new Mock<INotificationRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetNotificationsQueryHandler(
            _notificationRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotifications_When_TheyExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var notifications = new List<Notification>
            {
                new Notification(userId, "Test 1", NotificationType.Reminder),
                new Notification(userId, "Test 2", NotificationType.Achievement)
            };

        var notificationDtos = new List<NotificationDto>
            {
                new NotificationDto { Message = "Test 1" },
                new NotificationDto { Message = "Test 2" }
            };

        var query = new GetNotificationsQuery(userId);

        _notificationRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, null))
            .ReturnsAsync(notifications);

        _mapperMock.Setup(x => x.Map<IEnumerable<NotificationDto>>(notifications))
            .Returns(notificationDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Handle_Should_FilterByReadStatus_When_IsReadIsProvided()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetNotificationsQuery(userId, true);

        _notificationRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, true))
            .ReturnsAsync(new List<Notification>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _notificationRepositoryMock.Verify(x => x.GetByUserIdAsync(userId, true), Times.Once);
    }
}
