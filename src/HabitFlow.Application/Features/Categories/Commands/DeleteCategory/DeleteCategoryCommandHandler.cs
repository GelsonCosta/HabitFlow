using System;
using HabitFlow.Domain.Repositories;
using MediatR;

namespace HabitFlow.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IHabitRepository _habitRepository;

    public DeleteCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IHabitRepository habitRepository)
    {
        _categoryRepository = categoryRepository;
        _habitRepository = habitRepository;
    }

    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);

        if (category == null || category.UserId != request.UserId)
        {
            throw new ApplicationException("Categoria não encontrada ou não pertence ao usuário.");
        }

        
        var hasHabits = await _habitRepository.AnyByCategoryIdAsync(request.CategoryId);
        if (hasHabits)
        {
            throw new ApplicationException("Não é possível excluir a categoria pois há hábitos associados a ela.");
        }

        await _categoryRepository.DeleteAsync(category);

        return Unit.Value;
    }
}