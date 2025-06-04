using System;

namespace HabitFlow.Application.Features.Categories.Commands.CreateCategory.Dtos;

public class CreatedCategoryDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Color { get; set; }
    public DateTime CreationDate { get; set; }
}
