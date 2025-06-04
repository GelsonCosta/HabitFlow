using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsDone.Dtos;
using HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsNotDone.Dtos;
using HabitFlow.Application.Features.Habits.Commands.CreateHabit.Dtos;
using HabitFlow.Application.Features.Users.Commands.LoginUser.Dtos;
using HabitFlow.Application.Features.Users.Commands.RegisterUser.Dtos;
using HabitFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HabitFlow.IntegrationTests.Controllers;

public class HabitRecordsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private string _authToken;
    private Guid _habitId;

    public HabitRecordsControllerTests(WebApplicationFactory<Program> factory)
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

    private async Task InitializeTestData()
    {
        // Registrar e autenticar usuário
        var registerDto = new RegisterUserDto
        {
            Name = "Test User",
            Email = "test@habit.com",
            Password = "Test@123"
        };

        await _client.PostAsJsonAsync("/auth/register", registerDto);

        var loginDto = new LoginUserDto
        {
            Email = "test@habit.com",
            Password = "Test@123"
        };

        var loginResponse = await _client.PostAsJsonAsync("/auth/login", loginDto);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoggedInUserDto>();

        _authToken = loginResult.Token;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);

        // Criar um hábito para teste
        var habitDto = new CreateHabitDto
        {
            Name = "Test Habit",
            Frequency = "daily",
            Target = "1 time"
        };

        var habitResponse = await _client.PostAsJsonAsync("/habits", habitDto);
        var habitResult = await habitResponse.Content.ReadFromJsonAsync<CreatedHabitDto>();
        _habitId = habitResult.Id;
    }

    [Fact]
    public async Task MarkHabitAsDone_Should_ReturnHabitRecord_When_InputIsValid()
    {
        // Arrange
        await InitializeTestData();

        var recordDto = new MarkHabitAsDoneDto
        {
            Date = DateTime.Today,
            Note = "Test note",
            AchievedValue = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/habits/{_habitId}/records/done", recordDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<HabitRecordDto>();

        Assert.NotNull(result);
        Assert.Equal("Done", result.Status);
        Assert.Equal(recordDto.Note, result.Note);
        Assert.Equal(recordDto.AchievedValue, result.AchievedValue);
    }

    [Fact]
    public async Task MarkHabitAsDone_Should_ReturnNotFound_When_HabitDoesNotExist()
    {
        // Arrange
        await InitializeTestData();
        var nonExistentHabitId = Guid.NewGuid();

        var recordDto = new MarkHabitAsDoneDto { Date = DateTime.Today };

        // Act
        var response = await _client.PostAsJsonAsync($"/habits/{nonExistentHabitId}/records/done", recordDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task MarkHabitAsDone_Should_ReturnUnauthorized_When_UserIsNotAuthenticated()
    {
        // Arrange
        var recordDto = new MarkHabitAsDoneDto { Date = DateTime.Today };
        var anyHabitId = Guid.NewGuid();

        // Act
        var response = await _client.PostAsJsonAsync($"/habits/{anyHabitId}/records/done", recordDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    [Fact]
    public async Task MarkHabitAsNotDone_Should_ReturnHabitRecord_When_InputIsValid()
    {
        // Arrange
        await InitializeTestData();

        var recordDto = new MarkHabitAsNotDoneDto
        {
            Date = DateTime.Today,
            Note = "Esqueci de fazer hoje"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/habits/{_habitId}/records/not-done", recordDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<HabitRecordDto>();

        Assert.NotNull(result);
        Assert.Equal("NotDone", result.Status);
        Assert.Equal(recordDto.Note, result.Note);
        Assert.Null(result.AchievedValue);
    }

    [Fact]
    public async Task MarkHabitAsNotDone_Should_ReturnNotFound_When_HabitDoesNotExist()
    {
        // Arrange
        await InitializeTestData();
        var nonExistentHabitId = Guid.NewGuid();

        var recordDto = new MarkHabitAsNotDoneDto { Date = DateTime.Today };

        // Act
        var response = await _client.PostAsJsonAsync($"/habits/{nonExistentHabitId}/records/not-done", recordDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task MarkHabitAsNotDone_Should_ReturnUnauthorized_When_UserIsNotAuthenticated()
    {
        // Arrange
        var recordDto = new MarkHabitAsNotDoneDto { Date = DateTime.Today };
        var anyHabitId = Guid.NewGuid();

        // Act
        var response = await _client.PostAsJsonAsync($"/habits/{anyHabitId}/records/not-done", recordDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
