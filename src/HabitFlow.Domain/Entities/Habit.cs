using System;

namespace HabitFlow.Domain.Entities;

public class Habit : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Frequency { get; private set; }
    public string Target { get; private set; }
    public string Color { get; private set; }
    public DateTime CreationDate { get; private set; }

    
    private Habit() { }

    public Habit(
        Guid userId,
        string name,
        string frequency,
        string target,
        string description = null,
        Guid? categoryId = null,
        string color = null)
    {
        UserId = userId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Frequency = frequency ?? throw new ArgumentNullException(nameof(frequency));
        Target = target ?? throw new ArgumentNullException(nameof(target));
        Description = description;
        CategoryId = categoryId;
        Color = color;
        CreationDate = DateTime.UtcNow;
    }

   
    public void Update(
        string name,
        string frequency,
        string target,
        string description = null,
        Guid? categoryId = null,
        string color = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Frequency = frequency ?? throw new ArgumentNullException(nameof(frequency));
        Target = target ?? throw new ArgumentNullException(nameof(target));
        Description = description;
        CategoryId = categoryId;
        Color = color;
    }
}