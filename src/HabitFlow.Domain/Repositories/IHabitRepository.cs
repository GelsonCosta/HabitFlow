using System;
using HabitFlow.Domain.Entities;

namespace HabitFlow.Domain.Repositories;

public interface IHabitRepository
{
    Task<Habit> GetByIdAsync(Guid id);
    Task<IEnumerable<Habit>> GetByUserIdAsync(Guid userId);
    Task AddAsync(Habit habit);
    Task UpdateAsync(Habit habit);
    Task DeleteAsync(Habit habit);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> AnyByCategoryIdAsync(Guid categoryId);
    Task<IEnumerable<Habit>> GetHabitsAsync();
}
