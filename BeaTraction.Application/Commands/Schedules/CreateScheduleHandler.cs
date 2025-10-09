using BeaTraction.Application.DTOs.Schedules.Response;
using BeaTraction.Domain.Entities;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Schedules;

public class CreateScheduleHandler : IRequestHandler<CreateScheduleCommand, ScheduleDto>
{
    private readonly IScheduleRepository _scheduleRepository;

    public CreateScheduleHandler(IScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository;
    }

    public async Task<ScheduleDto> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = new Schedule
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        await _scheduleRepository.AddAsync(schedule, cancellationToken);

        return new ScheduleDto
        {
            Id = schedule.Id,
            Name = schedule.Name,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime
        };
    }
}
