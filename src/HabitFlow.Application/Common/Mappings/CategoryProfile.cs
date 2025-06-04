using System;
using AutoMapper;
using HabitFlow.Application.Features.Categories.Commands.CreateCategory.Dtos;
using HabitFlow.Application.Features.Categories.Queries.GetCategories.Dtos;
using HabitFlow.Domain.Entities;

namespace HabitFlow.Application.Common.Mappings;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CreatedCategoryDto>();
        CreateMap<Category, CategoryDto>();
    }
}
