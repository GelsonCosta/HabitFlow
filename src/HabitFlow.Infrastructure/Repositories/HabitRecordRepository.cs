using System;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Repositories;
using HabitFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HabitFlow.Infrastructure.Repositories;

public class HabitRecordRepository : IHabitRecordRepository
{
    private readonly ApplicationDbContext _context;

    public HabitRecordRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HabitRecord> GetByIdAsync(Guid id)
    {
        return await _context.HabitRecords.FindAsync(id);
    }

    public async Task<HabitRecord> GetByHabitAndDateAsync(Guid habitId, DateTime date)
    {
        return await _context.HabitRecords
            .FirstOrDefaultAsync(r => r.HabitId == habitId && r.Date.Date == date.Date);
    }

    public async Task<IEnumerable<HabitRecord>> GetByHabitIdAsync(Guid habitId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.HabitRecords
            .Where(r => r.HabitId == habitId);

        if (startDate.HasValue)
        {
            query = query.Where(r => r.Date >= startDate.Value.Date);
        }

        if (endDate.HasValue)
        {
            query = query.Where(r => r.Date <= endDate.Value.Date);
        }

        return await query.ToListAsync();
    }

    public async Task AddAsync(HabitRecord record)
    {
        await _context.HabitRecords.AddAsync(record);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(HabitRecord record)
    {
        _context.HabitRecords.Update(record);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid habitId, DateTime date)
    {
        return await _context.HabitRecords
            .AnyAsync(r => r.HabitId == habitId && r.Date.Date == date.Date);
    }
    public async Task DeleteAsync(HabitRecord record)
    {
        _context.HabitRecords.Remove(record);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<HabitRecord>> GetAllByHabitIdOrderedDescAsync(Guid habitId)
    {
        return await _context.HabitRecords
            .Where(r => r.HabitId == habitId)
            .OrderByDescending(r => r.Date)
            .ToListAsync();
    }

}