using System;

namespace HabitFlow.Application.Features.Categories.Queries.GetCategories.Dtos;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Color { get; set; }
    public DateTime CreationDate { get; set; }
}