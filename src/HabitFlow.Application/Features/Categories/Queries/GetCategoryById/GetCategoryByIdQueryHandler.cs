using System;
using AutoMapper;
using HabitFlow.Application.Features.Categories.Queries.GetCategories.Dtos;
using HabitFlow.Domain.Repositories;
using MediatR;

namespace HabitFlow.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetCategoryByIdQueryHandler(
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);

        if (category == null || category.UserId != request.UserId)
        {
            throw new ApplicationException("Categoria não encontrada ou não pertence ao usuário.");
        }

        return _mapper.Map<CategoryDto>(category);
    }
}
