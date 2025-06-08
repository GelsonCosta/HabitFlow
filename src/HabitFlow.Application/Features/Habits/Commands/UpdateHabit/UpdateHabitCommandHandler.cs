using System;
using AutoMapper;
using HabitFlow.Application.Features.Habits.Commands.UpdateHabit.Dtos;
using HabitFlow.Domain.Repositories;
using MediatR;

namespace HabitFlow.Application.Features.Habits.Commands.UpdateHabit;

public class UpdateHabitCommandHandler : IRequestHandler<UpdateHabitCommand, UpdatedHabitDto>
{
    private readonly IHabitRepository _habitRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public UpdateHabitCommandHandler(
        IHabitRepository habitRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _habitRepository = habitRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<UpdatedHabitDto> Handle(UpdateHabitCommand request, CancellationToken cancellationToken)
    {
        var habit = await _habitRepository.GetByIdAsync(request.HabitId);

        if (habit == null || habit.UserId != request.UserId)
        {
            throw new ApplicationException("Hábito não encontrado ou não pertence ao usuário.");
        }

        // Verificar se a categoria existe (se fornecida)
        if (request.HabitDto.CategoryId.HasValue)
        {
            var categoryExists = await _categoryRepository.ExistsAsync(request.HabitDto.CategoryId.Value, request.UserId);
            if (!categoryExists)
            {
                throw new ApplicationException("Categoria não encontrada ou não pertence ao usuário.");
            }
        }

        habit.Update(
            request.HabitDto.Name,
            request.HabitDto.Frequency,
            request.HabitDto.Target,
            request.HabitDto.Description,
            request.HabitDto.CategoryId,
            request.HabitDto.Color);

        await _habitRepository.UpdateAsync(habit);

        return _mapper.Map<UpdatedHabitDto>(habit);
    }
}
