using System;
using HabitFlow.Domain.Entities;

namespace HabitFlow.Domain.Repositories;

public interface IHabitRecordRepository
{
    Task<HabitRecord> GetByIdAsync(Guid id);
    Task<HabitRecord> GetByHabitAndDateAsync(Guid habitId, DateTime date);
    Task<IEnumerable<HabitRecord>> GetByHabitIdAsync(Guid habitId, DateTime? startDate = null, DateTime? endDate = null);
    Task AddAsync(HabitRecord record);
    Task UpdateAsync(HabitRecord record);
    Task<bool> ExistsAsync(Guid habitId, DateTime date);
    Task DeleteAsync(HabitRecord record);
    Task<IEnumerable<HabitRecord>> GetAllByHabitIdOrderedDescAsync(Guid habitId);

}
