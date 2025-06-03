using System;
using HabitFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitFlow.Infrastructure.Persistence.Configurations;

public class HabitRecordConfiguration : IEntityTypeConfiguration<HabitRecord>
{
    public void Configure(EntityTypeBuilder<HabitRecord> builder)
    {
        builder.ToTable("HabitRecords");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Date)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(r => r.Note)
            .HasMaxLength(1000);

        builder.Property(r => r.AchievedValue)
            .HasColumnType("decimal(18,2)");

        // Relacionamento com Habit
        builder.HasOne<Habit>()
            .WithMany()
            .HasForeignKey(r => r.HabitId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índice para evitar registros duplicados no mesmo dia
        builder.HasIndex(r => new { r.HabitId, r.Date })
            .IsUnique();
    }
}