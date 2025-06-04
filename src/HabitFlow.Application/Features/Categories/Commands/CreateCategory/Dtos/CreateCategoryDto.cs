using System;

namespace HabitFlow.Application.Features.Categories.Commands.CreateCategory.Dtos;

public class CreateCategoryDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Color { get; set; }
}
