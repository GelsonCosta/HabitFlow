using System;
using AutoMapper;
using HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsDone.Dtos;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Enums;
using HabitFlow.Domain.Events;
using HabitFlow.Domain.Repositories;
using MediatR;

namespace HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsDone;

public class MarkHabitAsDoneCommandHandler(
    IHabitRepository habitRepository,
    IHabitRecordRepository habitRecordRepository,
    IMapper mapper,
    IMediator mediator) : IRequestHandler<MarkHabitAsDoneCommand, HabitRecordDto>
{
    private readonly IHabitRepository _habitRepository = habitRepository;
    private readonly IHabitRecordRepository _habitRecordRepository = habitRecordRepository;
    private readonly IMapper _mapper = mapper;
    private IMediator _mediator = mediator;
    public async Task<HabitRecordDto> Handle(MarkHabitAsDoneCommand request, CancellationToken cancellationToken)
    {
        var habit = await _habitRepository.GetByIdAsync(request.HabitId);

        // Verificar se o hábito existe e pertence ao usuário
        if (habit == null || habit.UserId != request.UserId)
        {
            throw new ApplicationException("Hábito não encontrado ou não pertence ao usuário.");
        }

        var recordDto = request.RecordDto;
        var existingRecord = await _habitRecordRepository.GetByHabitAndDateAsync(request.HabitId, recordDto.Date);

        HabitRecord record;

        if (existingRecord != null)
        {
            // Atualizar registro existente
            existingRecord.Update(HabitStatus.Done, recordDto.Note, recordDto.AchievedValue);
            await _habitRecordRepository.UpdateAsync(existingRecord);
            record = existingRecord;
        }
        else
        {
            // Criar novo registro
            record = new HabitRecord(
                request.HabitId,
                recordDto.Date,
                HabitStatus.Done,
                recordDto.Note,
                recordDto.AchievedValue);

            await _habitRecordRepository.AddAsync(record);
        }

        var streakLength = await CalculateStreakLength(request.HabitId);
        await _mediator.Publish(new HabitMarkedAsDoneEvent(
            request.UserId,
            request.HabitId,
            habit.Name,
            streakLength));

        return _mapper.Map<HabitRecordDto>(record);
    }
    private async Task<int> CalculateStreakLength(Guid habitId)
    {
        var records = await _habitRecordRepository.GetAllByHabitIdOrderedDescAsync(habitId);

        int streak = 0;
        DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        foreach (var record in records)
        {
            if (DateOnly.FromDateTime(record.Date) != currentDate || record.Status != HabitStatus.Done)

            {
                break;
            }

            streak++;
            currentDate = currentDate.AddDays(-1);
        }

        return streak;
    }

}
