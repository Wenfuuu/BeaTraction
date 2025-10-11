namespace BeaTraction.Domain.Events.Attractions;

public class AttractionDeletedEvent : IDomainEvent
{
    public Guid AttractionId { get; }
    public DateTime OccurredOn { get; }

    public AttractionDeletedEvent(Guid attractionId)
    {
        AttractionId = attractionId;
        OccurredOn = DateTime.UtcNow;
    }
}
