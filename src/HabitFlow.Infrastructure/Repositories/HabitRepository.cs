using System;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Repositories;
using HabitFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HabitFlow.Infrastructure.Repositories;

public class HabitRepository : IHabitRepository
{
    private readonly ApplicationDbContext _context;

    public HabitRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Habit> GetByIdAsync(Guid id)
    {
        return await _context.Habits.FindAsync(id);
    }

    public async Task<IEnumerable<Habit>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Habits
            .Where(h => h.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(Habit habit)
    {
        await _context.Habits.AddAsync(habit);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Habit habit)
    {
        _context.Habits.Update(habit);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Habit habit)
    {
        _context.Habits.Remove(habit);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Habits.AnyAsync(h => h.Id == id);
    }
    public async Task<bool> AnyByCategoryIdAsync(Guid categoryId)
    {
        return await _context.Habits
            .AnyAsync(h => h.CategoryId == categoryId);
    }
}
