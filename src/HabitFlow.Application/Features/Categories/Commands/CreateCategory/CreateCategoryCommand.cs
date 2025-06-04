using System;
using HabitFlow.Application.Features.Categories.Commands.CreateCategory.Dtos;
using MediatR;

namespace HabitFlow.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommand : IRequest<CreatedCategoryDto>
{
    public Guid UserId { get; }
    public CreateCategoryDto CategoryDto { get; }

    public CreateCategoryCommand(Guid userId, CreateCategoryDto categoryDto)
    {
        UserId = userId;
        CategoryDto = categoryDto;
    }
}
