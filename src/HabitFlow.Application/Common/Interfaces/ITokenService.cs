using System;
using HabitFlow.Domain.Entities;

namespace HabitFlow.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
