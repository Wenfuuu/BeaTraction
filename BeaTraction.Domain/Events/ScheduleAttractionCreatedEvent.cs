namespace BeaTraction.Domain.Events;

public class ScheduleAttractionCreatedEvent : IDomainEvent
{
    public Guid ScheduleAttractionId { get; }
    public Guid ScheduleId { get; }
    public Guid AttractionId { get; }
    public DateTime OccurredOn { get; }

    public ScheduleAttractionCreatedEvent(
        Guid scheduleAttractionId,
        Guid scheduleId,
        Guid attractionId)
    {
        ScheduleAttractionId = scheduleAttractionId;
        ScheduleId = scheduleId;
        AttractionId = attractionId;
        OccurredOn = DateTime.UtcNow;
    }
}
