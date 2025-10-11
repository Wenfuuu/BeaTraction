namespace BeaTraction.Domain.Events.Attractions;

public class AttractionCreatedEvent : IDomainEvent
{
    public Guid AttractionId { get; }
    public string Name { get; }
    public int Capacity { get; }
    public DateTime OccurredOn { get; }

    public AttractionCreatedEvent(Guid attractionId, string name, int capacity)
    {
        AttractionId = attractionId;
        Name = name;
        Capacity = capacity;
        OccurredOn = DateTime.UtcNow;
    }
}
