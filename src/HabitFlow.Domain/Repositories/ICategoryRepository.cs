using System;
using HabitFlow.Domain.Entities;

namespace HabitFlow.Domain.Repositories;

public interface ICategoryRepository
{
    Task AddAsync(Category category);
    Task<bool> ExistsAsync(Guid value, Guid userId);
}
