using BeaTraction.Domain.Events.ScheduleAttractions;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.ScheduleAttractions;

public class DeleteScheduleAttractionHandler : IRequestHandler<DeleteScheduleAttractionCommand, bool>
{
    private readonly IScheduleAttractionRepository _scheduleAttractionRepository;
    private readonly IPublisher _publisher;

    public DeleteScheduleAttractionHandler(
        IScheduleAttractionRepository scheduleAttractionRepository,
        IPublisher publisher)
    {
        _scheduleAttractionRepository = scheduleAttractionRepository;
        _publisher = publisher;
    }

    public async Task<bool> Handle(DeleteScheduleAttractionCommand request, CancellationToken cancellationToken)
    {
        var scheduleAttraction = await _scheduleAttractionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (scheduleAttraction == null)
        {
            throw new InvalidOperationException("Schedule Attraction not found");
        }

        var scheduleAttractionId = scheduleAttraction.Id;
        var scheduleId = scheduleAttraction.ScheduleId;
        var attractionId = scheduleAttraction.AttractionId;

        await _scheduleAttractionRepository.DeleteAsync(scheduleAttraction, cancellationToken);

        var domainEvent = new ScheduleAttractionDeletedEvent(
            scheduleAttractionId,
            scheduleId,
            attractionId
        );
        await _publisher.Publish(domainEvent, cancellationToken);

        return true;
    }
}
