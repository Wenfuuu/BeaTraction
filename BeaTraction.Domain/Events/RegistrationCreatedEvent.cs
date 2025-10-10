namespace BeaTraction.Domain.Events;

public class RegistrationCreatedEvent : IDomainEvent
{
    public Guid RegistrationId { get; }
    public Guid UserId { get; }
    public Guid ScheduleAttractionId { get; }
    public DateTime RegisteredAt { get; }
    public DateTime OccurredOn { get; }

    public RegistrationCreatedEvent(
        Guid registrationId,
        Guid userId,
        Guid scheduleAttractionId,
        DateTime registeredAt)
    {
        RegistrationId = registrationId;
        UserId = userId;
        ScheduleAttractionId = scheduleAttractionId;
        RegisteredAt = registeredAt;
        OccurredOn = DateTime.UtcNow;
    }
}
