using System;
using AutoMapper;
using HabitFlow.Application.Features.Categories.Commands.CreateCategory.Dtos;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Repositories;
using MediatR;

namespace HabitFlow.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CreatedCategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<CreatedCategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _userRepository.ExistsAsync(request.UserId);
        if (!userExists)
        {
            throw new ApplicationException("Usuário não encontrado.");
        }

        var category = new Category(
            request.UserId,
            request.CategoryDto.Name,
            request.CategoryDto.Description,
            request.CategoryDto.Color);

        await _categoryRepository.AddAsync(category);

        return _mapper.Map<CreatedCategoryDto>(category);
    }
}
