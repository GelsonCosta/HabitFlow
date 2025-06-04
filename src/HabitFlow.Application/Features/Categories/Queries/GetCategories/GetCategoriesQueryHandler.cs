using System;
using AutoMapper;
using HabitFlow.Application.Features.Categories.Queries.GetCategories.Dtos;
using HabitFlow.Domain.Repositories;
using MediatR;

namespace HabitFlow.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IEnumerable<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetCategoriesQueryHandler(
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetByUserIdAsync(request.UserId);
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }
}
