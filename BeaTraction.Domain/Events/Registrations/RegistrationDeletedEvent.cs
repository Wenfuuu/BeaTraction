namespace BeaTraction.Domain.Events.Registrations;

public class RegistrationDeletedEvent : IDomainEvent
{
    public Guid RegistrationId { get; }
    public Guid UserId { get; }
    public Guid ScheduleAttractionId { get; }
    public DateTime OccurredOn { get; }

    public RegistrationDeletedEvent(
        Guid registrationId,
        Guid userId,
        Guid scheduleAttractionId)
    {
        RegistrationId = registrationId;
        UserId = userId;
        ScheduleAttractionId = scheduleAttractionId;
        OccurredOn = DateTime.UtcNow;
    }
}
