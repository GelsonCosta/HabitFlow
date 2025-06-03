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
    public async Task AddAsync(Category category)
    {


        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

    }

   public async Task<bool> ExistsAsync(Guid value, Guid userId)
    {
        return await _context.Categories
            .AnyAsync(c => c.Id == value && c.UserId == userId);
    }
}
