using System;
using System.Net;
using System.Net.Http.Json;
using HabitFlow.Application.Features.Users.Commands.LoginUser.Dtos;
using HabitFlow.Application.Features.Users.Commands.RegisterUser.Dtos;
using HabitFlow.Application.Features.Users.Commands.RegisterUser.Dtos.RegisteredUserDto;
using HabitFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace HabitFlow.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext configuration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Register_Should_ReturnCreated_When_InputIsValid()
    {
        // Arrange
        var userDto = new RegisterUserDto
        {
            Name = "Integration Test User",
            Email = "integration@test.com",
            Password = "Test@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", userDto);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<RegisteredUserDto>();
        Assert.NotNull(result);
        Assert.Equal(userDto.Name, result.Name);
        Assert.Equal(userDto.Email, result.Email);
    }

    [Fact]
    public async Task Register_Should_ReturnBadRequest_When_EmailAlreadyExists()
    {
        // Arrange
        var userDto = new RegisterUserDto
        {
            Name = "Duplicate Email User",
            Email = "duplicate@test.com",
            Password = "Test@123"
        };

        // First registration (should succeed)
        var firstResponse = await _client.PostAsJsonAsync("/auth/register", userDto);
        firstResponse.EnsureSuccessStatusCode();

        // Second registration with same email (should fail)
        var secondResponse = await _client.PostAsJsonAsync("/auth/register", userDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }

    [Theory]
    [InlineData(null, "test@email.com", "Test@123")]
    [InlineData("Test User", null, "Test@123")]
    [InlineData("Test User", "test@email.com", null)]
    [InlineData("Test User", "invalid-email", "Test@123")]
    [InlineData("Test User", "test@email.com", "short")]
    public async Task Register_Should_ReturnBadRequest_When_InputIsInvalid(string name, string email, string password)
    {
        // Arrange
        var userDto = new RegisterUserDto
        {
            Name = name,
            Email = email,
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", userDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    [Fact]
    public async Task Login_Should_ReturnTokenAndUser_When_CredentialsAreValid()
    {
        // Arrange - primeiro registrar um usuário
        var registerDto = new RegisterUserDto
        {
            Name = "Login Test User",
            Email = "login@test.com",
            Password = "Test@123"
        };

        await _client.PostAsJsonAsync("/auth/register", registerDto);

        // Act - fazer login
        var loginDto = new LoginUserDto
        {
            Email = "login@test.com",
            Password = "Test@123"
        };

        var response = await _client.PostAsJsonAsync("/auth/login", loginDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<LoggedInUserDto>();

        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.Equal(registerDto.Name, result.User.Name);
        Assert.Equal(registerDto.Email, result.User.Email);
    }

    [Fact]
    public async Task Login_Should_ReturnUnauthorized_When_UserNotFound()
    {
        // Arrange
        var loginDto = new LoginUserDto
        {
            Email = "nonexistent@email.com",
            Password = "any_password"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_Should_ReturnUnauthorized_When_PasswordIsIncorrect()
    {
        // Arrange - registrar usuário
        var registerDto = new RegisterUserDto
        {
            Name = "Wrong Password User",
            Email = "wrongpass@test.com",
            Password = "Correct@123"
        };

        await _client.PostAsJsonAsync("/auth/register", registerDto);

        // Act - tentar login com senha errada
        var loginDto = new LoginUserDto
        {
            Email = "wrongpass@test.com",
            Password = "Wrong@123"
        };

        var response = await _client.PostAsJsonAsync("/auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
