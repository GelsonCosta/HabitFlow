using System;
using HabitFlow.Domain.Entities;

namespace HabitFlow.Domain.Repositories;

public interface IUserRepository
{
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> ExistsAsync(Guid userId);
}
