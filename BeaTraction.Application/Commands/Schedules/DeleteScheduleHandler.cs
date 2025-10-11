using BeaTraction.Domain.Events;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Schedules;

public class DeleteScheduleHandler : IRequestHandler<DeleteScheduleCommand, bool>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IPublisher _publisher;

    public DeleteScheduleHandler(
        IScheduleRepository scheduleRepository,
        IPublisher publisher)
    {
        _scheduleRepository = scheduleRepository;
        _publisher = publisher;
    }

    public async Task<bool> Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (schedule == null)
        {
            throw new InvalidOperationException("Schedule not found");
        }

        await _scheduleRepository.DeleteAsync(schedule, cancellationToken);

        var domainEvent = new ScheduleDeletedEvent(schedule.Id);
        await _publisher.Publish(domainEvent, cancellationToken);

        return true;
    }
}
