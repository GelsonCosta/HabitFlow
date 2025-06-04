using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HabitFlow.Application.Features.Habits.Commands.CreateHabit.Dtos;
using HabitFlow.Application.Features.Habits.Queries.GetUserHabits;
using HabitFlow.Application.Features.Users.Commands.LoginUser.Dtos;
using HabitFlow.Application.Features.Users.Commands.RegisterUser.Dtos;
using HabitFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace HabitFlow.IntegrationTests.Controllers;

public class HabitsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private string _authToken;

    public HabitsControllerTests(WebApplicationFactory<Program> factory)
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
        // Registrar um usuário
        var registerDto = new RegisterUserDto
        {
            Name = "Test User",
            Email = "test@habit.com",
            Password = "Test@123"
        };

        await _client.PostAsJsonAsync("/auth/register", registerDto);

        // Fazer login
        var loginDto = new LoginUserDto
        {
            Email = "test@habit.com",
            Password = "Test@123"
        };

        var loginResponse = await _client.PostAsJsonAsync("/auth/login", loginDto);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoggedInUserDto>();

        _authToken = loginResult.Token;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
    }

    [Fact]
    public async Task CreateHabit_Should_ReturnUnauthorized_When_UserIsNotAuthenticated()
    {
        // Arrange
        var habitDto = new CreateHabitDto
        {
            Name = "Beber Água",
            Frequency = "daily",
            Target = "2 liters"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/habits", habitDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateHabit_Should_ReturnCreatedHabit_When_InputIsValid()
    {
        // Arrange
        await AuthenticateAsync();

        var habitDto = new CreateHabitDto
        {
            Name = "Beber Água",
            Description = "Beber 2 litros de água por dia",
            Frequency = "daily",
            Target = "2 liters",
            Color = "#4A90E2"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/habits", habitDto);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<CreatedHabitDto>();
        Assert.NotNull(result);
        Assert.Equal(habitDto.Name, result.Name);
        Assert.Equal(habitDto.Description, result.Description);
    }

    [Fact]
    public async Task CreateHabit_Should_ReturnBadRequest_When_NameIsMissing()
    {
        // Arrange
        await AuthenticateAsync();

        var habitDto = new CreateHabitDto
        {
            Frequency = "daily",
            Target = "2 liters"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/habits", habitDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    [Fact]
    public async Task GetUserHabits_Should_ReturnUserHabits_When_UserIsAuthenticated()
    {
        // Arrange
        await AuthenticateAsync();

        // Criar alguns hábitos
        var habit1 = new CreateHabitDto
        {
            Name = "Habit 1",
            Frequency = "daily",
            Target = "1 time"
        };

        var habit2 = new CreateHabitDto
        {
            Name = "Habit 2",
            Frequency = "weekly",
            Target = "3 times"
        };

        await _client.PostAsJsonAsync("/habits", habit1);
        await _client.PostAsJsonAsync("/habits", habit2);

        // Act
        var response = await _client.GetAsync("/habits");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<HabitDto>>();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetUserHabits_Should_ReturnEmptyList_When_NoHabitsExist()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await _client.GetAsync("/habits");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<HabitDto>>();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserHabits_Should_ReturnUnauthorized_When_UserIsNotAuthenticated()
    {
        // Act
        var response = await _client.GetAsync("/habits");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
