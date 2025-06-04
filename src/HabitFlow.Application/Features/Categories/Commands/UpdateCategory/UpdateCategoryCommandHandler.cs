using System;
using AutoMapper;
using HabitFlow.Application.Features.Categories.Queries.GetCategories.Dtos;
using HabitFlow.Domain.Repositories;
using MediatR;

namespace HabitFlow.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public UpdateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);

        if (category == null || category.UserId != request.UserId)
        {
            throw new ApplicationException("Categoria não encontrada ou não pertence ao usuário.");
        }

        category.Update(
            request.CategoryDto.Name,
            request.CategoryDto.Description,
            request.CategoryDto.Color);

        await _categoryRepository.UpdateAsync(category);

        return _mapper.Map<CategoryDto>(category);
    }
}