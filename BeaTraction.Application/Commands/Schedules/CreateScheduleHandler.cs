using BeaTraction.Application.DTOs.Schedules.Response;
using BeaTraction.Domain.Entities;
using BeaTraction.Domain.Events.Schedules;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Schedules;

public class CreateScheduleHandler : IRequestHandler<CreateScheduleCommand, ScheduleDto>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IPublisher _publisher;

    public CreateScheduleHandler(
        IScheduleRepository scheduleRepository,
        IPublisher publisher)
    {
        _scheduleRepository = scheduleRepository;
        _publisher = publisher;
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

        var domainEvent = new ScheduleCreatedEvent(
            schedule.Id,
            schedule.Name,
            schedule.StartTime,
            schedule.EndTime
        );
        await _publisher.Publish(domainEvent, cancellationToken);

        return new ScheduleDto
        {
            Id = schedule.Id,
            Name = schedule.Name,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime
        };
    }
}
