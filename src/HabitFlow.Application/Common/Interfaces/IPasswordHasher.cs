using System;

namespace HabitFlow.Application.Common.Interfaces;

public interface IPasswordHasher
{
    public string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
