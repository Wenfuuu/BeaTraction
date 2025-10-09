using BeaTraction.Application.DTOs.Schedules.Response;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Queries.Schedules;

public class GetAllSchedulesHandler : IRequestHandler<GetAllSchedulesQuery, List<ScheduleDto>>
{
    private readonly IScheduleRepository _scheduleRepository;

    public GetAllSchedulesHandler(IScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository;
    }

    public async Task<List<ScheduleDto>> Handle(GetAllSchedulesQuery request, CancellationToken cancellationToken)
    {
        var schedules = await _scheduleRepository.GetAllAsync(cancellationToken);
        
        return schedules.Select(s => new ScheduleDto
        {
            Id = s.Id,
            Name = s.Name,
            StartTime = s.StartTime,
            EndTime = s.EndTime
        }).ToList();
    }
}
