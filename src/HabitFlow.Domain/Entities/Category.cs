using System;

namespace HabitFlow.Domain.Entities
{
    public class Category : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Color { get; private set; }
        public DateTime CreationDate { get; private set; }

        // Construtor privado para EF Core
        private Category() { }

        public Category(
            Guid userId,
            string name,
            string description = null,
            string color = null)
        {
            UserId = userId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Color = color;
            CreationDate = DateTime.UtcNow;
        }

        
        public void Update(
            string name,
            string description = null,
            string color = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Color = color;
        }
    }
}