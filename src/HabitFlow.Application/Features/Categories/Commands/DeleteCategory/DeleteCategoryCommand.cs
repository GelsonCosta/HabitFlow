using System;
using MediatR;

namespace HabitFlow.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommand(Guid userId, Guid categoryId) : IRequest<Unit>
{
    public Guid UserId { get; } = userId;
    public Guid CategoryId { get; } = categoryId;
}