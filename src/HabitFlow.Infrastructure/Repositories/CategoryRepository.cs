using System;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Repositories;
using HabitFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HabitFlow.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Category> GetByIdAsync(Guid id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<IEnumerable<Category>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Categories
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id, Guid userId)
    {
        return await _context.Categories
            .AnyAsync(c => c.Id == id && c.UserId == userId);
    }

    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Category category)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
}