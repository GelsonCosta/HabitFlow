using System;

namespace HabitFlow.Application.Features.Categories.Commands.UpdateCategory.Dtos;

public class UpdateCategoryDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Color { get; set; }
}
