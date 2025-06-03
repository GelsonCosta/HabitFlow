using System;
using HabitFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HabitFlow.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Habit> Habits { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<HabitRecord> HabitRecords { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    }
