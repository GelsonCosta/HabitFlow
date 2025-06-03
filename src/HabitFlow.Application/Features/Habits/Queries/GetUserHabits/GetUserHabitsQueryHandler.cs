using System;
using AutoMapper;
using HabitFlow.Domain.Repositories;
using MediatR;

namespace HabitFlow.Application.Features.Habits.Queries.GetUserHabits;

public class GetUserHabitsQueryHandler : IRequestHandler<GetUserHabitsQuery, IEnumerable<HabitDto>>
{
    private readonly IHabitRepository _habitRepository;
    private readonly IMapper _mapper;

    public GetUserHabitsQueryHandler(
        IHabitRepository habitRepository,
        IMapper mapper)
    {
        _habitRepository = habitRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<HabitDto>> Handle(GetUserHabitsQuery request, CancellationToken cancellationToken)
    {
        var habits = await _habitRepository.GetByUserIdAsync(request.UserId);
        return _mapper.Map<IEnumerable<HabitDto>>(habits);
    }
}
