using System;
using HabitFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitFlow.Infrastructure.Persistence.Configurations;

public class HabitConfiguration : IEntityTypeConfiguration<Habit>
{
    public void Configure(EntityTypeBuilder<Habit> builder)
    {
        builder.ToTable("Habits");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(h => h.Description)
            .HasMaxLength(500);

        builder.Property(h => h.Frequency)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(h => h.Target)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(h => h.Color)
            .HasMaxLength(20);

        builder.Property(h => h.CreationDate)
            .IsRequired();

        // Relacionamento com User
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(h => h.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        
        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(h => h.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
