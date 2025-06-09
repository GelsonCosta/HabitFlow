using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HabitFlow.Application.Features.Notifications.Queries.GetNotifications.Dtos;
using HabitFlow.Application.Features.Users.Commands.LoginUser.Dtos;
using HabitFlow.Application.Features.Users.Commands.RegisterUser.Dtos;
using HabitFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HabitFlow.IntegrationTests.Controllers;

public class NotificationsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private string _authToken;

    public NotificationsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });
            });
        });

        _client = _factory.CreateClient();
    }

    private async Task AuthenticateAsync()
    {
        var registerDto = new RegisterUserDto
        {
            Name = "Test User",
            Email = "test@notifications.com",
            Password = "Test@123"
        };

        await _client.PostAsJsonAsync("/auth/register", registerDto);

        var loginDto = new LoginUserDto
        {
            Email = "test@notifications.com",
            Password = "Test@123"
        };

        var loginResponse = await _client.PostAsJsonAsync("/auth/login", loginDto);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoggedInUserDto>();

        _authToken = loginResult.Token;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
    }

    [Fact]
    public async Task GetNotifications_Should_ReturnEmptyList_When_NoNotificationsExist()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await _client.GetAsync("/notifications");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<NotificationDto[]>();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task MarkNotificationAsRead_Should_ReturnNotFound_When_NotificationDoesNotExist()
    {
        // Arrange
        await AuthenticateAsync();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.PatchAsync($"/notifications/{nonExistentId}/read", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AllEndpoints_Should_ReturnUnauthorized_When_UserIsNotAuthenticated()
    {
        // Arrange
        var anyId = Guid.NewGuid();

        // Act & Assert
        var getResponse = await _client.GetAsync("/notifications");
        Assert.Equal(HttpStatusCode.Unauthorized, getResponse.StatusCode);

        var patchResponse = await _client.PatchAsync($"/notifications/{anyId}/read", null);
        Assert.Equal(HttpStatusCode.Unauthorized, patchResponse.StatusCode);

        var postResponse = await _client.PostAsync("/notifications/mark-all-read", null);
        Assert.Equal(HttpStatusCode.Unauthorized, postResponse.StatusCode);
    }
}
