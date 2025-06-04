using System;
using HabitFlow.Domain.Entities;

namespace HabitFlow.Domain.Repositories;

public interface ICategoryRepository
{
        Task<Category> GetByIdAsync(Guid id);
        Task<IEnumerable<Category>> GetByUserIdAsync(Guid userId);
        Task<bool> ExistsAsync(Guid id, Guid userId);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
}
