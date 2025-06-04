using System;
using HabitFlow.Application.Features.Categories.Queries.GetCategories.Dtos;
using MediatR;

namespace HabitFlow.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQuery : IRequest<IEnumerable<CategoryDto>>
{
    public Guid UserId { get; }

    public GetCategoriesQuery(Guid userId)
    {
        UserId = userId;
    }
}
