using System;
using HabitFlow.Application.Features.Categories.Queries.GetCategories.Dtos;
using MediatR;

namespace HabitFlow.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQuery : IRequest<CategoryDto>
{
    public Guid UserId { get; }
    public Guid CategoryId { get; }

    public GetCategoryByIdQuery(Guid userId, Guid categoryId)
    {
        UserId = userId;
        CategoryId = categoryId;
    }
}