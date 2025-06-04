using System;
using HabitFlow.Domain.Entities;
using HabitFlow.Infrastructure.Persistence;
using HabitFlow.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HabitFlow.IntegrationTests.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "UserRepositoryTestDb")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new UserRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task AddAsync_Should_AddUserToDatabase()
    {
        // Arrange
        var user = new User("Test User", "test@email.com", "hashed_password");

        // Act
        await _repository.AddAsync(user);
        var result = await _repository.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_Should_ReturnUser_When_EmailExists()
    {
        // Arrange
        var email = "existing@email.com";
        var user = new User("Existing User", email, "hashed_password");
        await _repository.AddAsync(user);

        // Act
        var result = await _repository.GetByEmailAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_Should_ReturnNull_When_EmailDoesNotExist()
    {
        // Arrange
        var email = "nonexisting@email.com";

        // Act
        var result = await _repository.GetByEmailAsync(email);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task EmailExistsAsync_Should_ReturnTrue_When_EmailExists()
    {
        // Arrange
        var email = "exists@email.com";
        var user = new User("Existing User", email, "hashed_password");
        await _repository.AddAsync(user);

        // Act
        var result = await _repository.EmailExistsAsync(email);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task EmailExistsAsync_Should_ReturnFalse_When_EmailDoesNotExist()
    {
        // Arrange
        var email = "doesnotexist@email.com";

        // Act
        var result = await _repository.EmailExistsAsync(email);

        // Assert
        Assert.False(result);
    }
}
