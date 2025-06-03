using System;
using HabitFlow.Application.Features.Habits.Commands.CreateHabit.Dtos;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Repositories;
using MediatR;

namespace HabitFlow.Application.Features.Habits.Commands;

public class CreateHabitCommandHandler : IRequestHandler<CreateHabitCommand, CreatedHabitDto>
{
    private readonly IHabitRepository _habitRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateHabitCommandHandler(
        IHabitRepository habitRepository,
        IUserRepository userRepository,
        ICategoryRepository categoryRepository)
    {
        _habitRepository = habitRepository;
        _userRepository = userRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<CreatedHabitDto> Handle(CreateHabitCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var habitDto = request.HabitDto;

        // Verificar se o usuário existe
        var userExists = await _userRepository.ExistsAsync(userId);
        if (!userExists)
        {
            throw new ApplicationException("Usuário não encontrado.");
        }

        // Verificar se a categoria existe (se fornecida)
        if (habitDto.CategoryId.HasValue)
        {
            var categoryExists = await _categoryRepository.ExistsAsync(habitDto.CategoryId.Value, userId);
            if (!categoryExists)
            {
                throw new ApplicationException("Categoria não encontrada ou não pertence ao usuário.");
            }
        }

        // Criar o novo hábito
        var habit = new Habit(
            userId,
            habitDto.Name,
            habitDto.Frequency,
            habitDto.Target,
            habitDto.Description,
            habitDto.CategoryId,
            habitDto.Color);

        await _habitRepository.AddAsync(habit);

        return new CreatedHabitDto
        {
            Id = habit.Id,
            UserId = habit.UserId,
            CategoryId = habit.CategoryId,
            Name = habit.Name,
            Description = habit.Description,
            Frequency = habit.Frequency,
            Target = habit.Target,
            Color = habit.Color,
            CreationDate = habit.CreationDate
        };
    }
}