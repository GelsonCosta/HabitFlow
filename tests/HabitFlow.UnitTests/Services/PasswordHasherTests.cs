using System;
using HabitFlow.Infrastructure.Services;

namespace HabitFlow.UnitTests.Services;

public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher;

    public PasswordHasherTests()
    {
        _passwordHasher = new PasswordHasher();
    }

    [Fact]
    public void HashPassword_Should_ReturnDifferentHash_ForDifferentPasswords()
    {
        // Arrange
        var password1 = "Password1";
        var password2 = "Password2";

        // Act
        var hash1 = _passwordHasher.HashPassword(password1);
        var hash2 = _passwordHasher.HashPassword(password2);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void HashPassword_Should_ReturnConsistentHash_ForSamePassword()
    {
        // Arrange
        var password = "ConsistentPassword";

        // Act
        var hash1 = _passwordHasher.HashPassword(password);
        var hash2 = _passwordHasher.HashPassword(password);

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void VerifyPassword_Should_ReturnTrue_ForCorrectPassword()
    {
        // Arrange
        var password = "CorrectPassword";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(password, hash);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_Should_ReturnFalse_ForIncorrectPassword()
    {
        // Arrange
        var correctPassword = "CorrectPassword";
        var incorrectPassword = "IncorrectPassword";
        var hash = _passwordHasher.HashPassword(correctPassword);

        // Act
        var result = _passwordHasher.VerifyPassword(incorrectPassword, hash);

        // Assert
        Assert.False(result);
    }
}
