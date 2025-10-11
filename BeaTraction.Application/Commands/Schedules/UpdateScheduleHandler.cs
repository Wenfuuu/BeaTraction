using BeaTraction.Application.DTOs.Schedules.Response;
using BeaTraction.Domain.Events;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Schedules;

public class UpdateScheduleHandler : IRequestHandler<UpdateScheduleCommand, ScheduleDto>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IPublisher _publisher;

    public UpdateScheduleHandler(
        IScheduleRepository scheduleRepository,
        IPublisher publisher)
    {
        _scheduleRepository = scheduleRepository;
        _publisher = publisher;
    }

    public async Task<ScheduleDto> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (schedule == null)
        {
            throw new InvalidOperationException("Schedule not found");
        }

        schedule.Name = request.Name;
        schedule.StartTime = request.StartTime;
        schedule.EndTime = request.EndTime;

        await _scheduleRepository.UpdateAsync(schedule, cancellationToken);

        var domainEvent = new ScheduleUpdatedEvent(
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
