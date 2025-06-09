using System;
using HabitFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitFlow.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(n => n.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(n => n.SendDate)
            .IsRequired();

        builder.Property(n => n.IsRead)
            .IsRequired();

        // Relacionamento com User
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento opcional com Habit
        builder.HasOne<Habit>()
            .WithMany()
            .HasForeignKey(n => n.HabitId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
