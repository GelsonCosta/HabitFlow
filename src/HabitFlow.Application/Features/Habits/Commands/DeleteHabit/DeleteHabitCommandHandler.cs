using System;
using HabitFlow.Domain.Repositories;
using MediatR;

namespace HabitFlow.Application.Features.Habits.Commands.DeleteHabit;

public class DeleteHabitCommandHandler : IRequestHandler<DeleteHabitCommand, Unit>
{
    private readonly IHabitRepository _habitRepository;
    private readonly IHabitRecordRepository _habitRecordRepository;

    public DeleteHabitCommandHandler(
        IHabitRepository habitRepository,
        IHabitRecordRepository habitRecordRepository)
    {
        _habitRepository = habitRepository;
        _habitRecordRepository = habitRecordRepository;
    }

    public async Task<Unit> Handle(DeleteHabitCommand request, CancellationToken cancellationToken)
    {
        var habit = await _habitRepository.GetByIdAsync(request.HabitId);

        if (habit == null || habit.UserId != request.UserId)
        {
            throw new ApplicationException("Hábito não encontrado ou não pertence ao usuário.");
        }

        // Primeiro deletar todos os registros associados
        var records = await _habitRecordRepository.GetByHabitIdAsync(request.HabitId);
        foreach (var record in records)
        {
            await _habitRecordRepository.DeleteAsync(record);
        }

        // Depois deletar o hábito
        await _habitRepository.DeleteAsync(habit);

        return Unit.Value;
    }
}
