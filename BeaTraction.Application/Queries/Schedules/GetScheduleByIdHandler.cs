using BeaTraction.Application.DTOs.Schedules.Response;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Queries.Schedules;

public class GetScheduleByIdHandler : IRequestHandler<GetScheduleByIdQuery, ScheduleDto>
{
    private readonly IScheduleRepository _scheduleRepository;

    public GetScheduleByIdHandler(IScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository;
    }

    public async Task<ScheduleDto> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(request.Id);
        if (schedule == null)
        {
            throw new InvalidOperationException("Schedule not found");
        }

        return new ScheduleDto
        {
            Id = schedule.Id,
            AttractionId = schedule.AttractionId,
            Name = schedule.Name,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime
        };
    }
}
