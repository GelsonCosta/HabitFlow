using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsDone.Dtos;
using HabitFlow.Application.Features.Habits.Commands.CreateHabit.Dtos;
using HabitFlow.Application.Features.Habits.Commands.UpdateHabit.Dtos;
using HabitFlow.Application.Features.Users.Commands.LoginUser.Dtos;
using HabitFlow.Application.Features.Users.Commands.RegisterUser.Dtos;
using HabitFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HabitFlow.IntegrationTests.Controllers;

public class HabitsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private string _authToken;
    private Guid _habitId;

    public HabitsControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
            Email = "test@habits.com",
            Password = "Test@123"
        };

        await _client.PostAsJsonAsync("/auth/register", registerDto);

        var loginDto = new LoginUserDto
        {
            Email = "test@habits.com",
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
    public async Task UpdateHabit_Should_ReturnUpdatedHabit_When_InputIsValid()
    {
        // Arrange
        await InitializeTestData();

        var updateDto = new UpdateHabitDto
        {
            Name = "Updated Name",
            Description = "Updated Desc",
            Frequency = "weekly",
            Target = "2 times",
            Color = "#FFFFFF"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/habits/{_habitId}", updateDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<UpdatedHabitDto>();

        Assert.NotNull(result);
        Assert.Equal(updateDto.Name, result.Name);
        Assert.Equal(updateDto.Description, result.Description);
        Assert.Equal(updateDto.Frequency, result.Frequency);
        Assert.Equal(updateDto.Target, result.Target);
        Assert.Equal(updateDto.Color, result.Color);
    }

    [Fact]
    public async Task DeleteHabit_Should_ReturnNoContent_When_HabitExists()
    {
        // Arrange
        await InitializeTestData();

        // Act
        var response = await _client.DeleteAsync($"/habits/{_habitId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify deleted
        var getResponse = await _client.GetAsync($"/habits/{_habitId}");
        Assert.Equal(HttpStatusCode.BadRequest, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteHabit_Should_DeleteAssociatedRecords()
    {
        // Arrange
        await InitializeTestData();

        // Criar registro para o hábito
        var recordDto = new MarkHabitAsDoneDto { Date = DateTime.Today };
        await _client.PostAsJsonAsync($"/habits/{_habitId}/records/done", recordDto);

        // Act
        var deleteResponse = await _client.DeleteAsync($"/habits/{_habitId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Tentar obter registros (deve falhar)
        var getRecordsResponse = await _client.GetAsync($"/habits/{_habitId}/records");
        Assert.Equal(HttpStatusCode.BadRequest, getRecordsResponse.StatusCode);
    }
}