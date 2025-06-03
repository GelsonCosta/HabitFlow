using System;
using AutoMapper;
using HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsDone.Dtos;
using HabitFlow.Domain.Entities;
using HabitFlow.Domain.Enums;
using HabitFlow.Domain.Repositories;
using MediatR;

namespace HabitFlow.Application.Features.HabitRecords.Commands.MarkHabitAsDone;

public class MarkHabitAsDoneCommandHandler : IRequestHandler<MarkHabitAsDoneCommand, HabitRecordDto>
{
    private readonly IHabitRepository _habitRepository;
    private readonly IHabitRecordRepository _habitRecordRepository;
    private readonly IMapper _mapper;

    public MarkHabitAsDoneCommandHandler(
        IHabitRepository habitRepository,
        IHabitRecordRepository habitRecordRepository,
        IMapper mapper)
    {
        _habitRepository = habitRepository;
        _habitRecordRepository = habitRecordRepository;
        _mapper = mapper;
    }

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

        return _mapper.Map<HabitRecordDto>(record);
    }
}
