using System;
using HabitFlow.Application.Features.Categories.Commands.UpdateCategory.Dtos;
using HabitFlow.Application.Features.Categories.Queries.GetCategories.Dtos;
using MediatR;

namespace HabitFlow.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest<CategoryDto>
{
    public Guid UserId { get; }
    public Guid CategoryId { get; }
    public UpdateCategoryDto CategoryDto { get; }

    public UpdateCategoryCommand(
        Guid userId,
        Guid categoryId,
        UpdateCategoryDto categoryDto)
    {
        UserId = userId;
        CategoryId = categoryId;
        CategoryDto = categoryDto;
    }
}
