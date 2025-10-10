namespace BeaTraction.Application.Interfaces;

public interface IRealtimeNotificationService
{
    Task NotifyRegistrationCreatedAsync(Guid registrationId, Guid userId, Guid scheduleAttractionId, DateTime registeredAt, DateTime occurredOn);
    Task NotifyRegistrationDeletedAsync(Guid registrationId, Guid userId, Guid scheduleAttractionId, DateTime occurredOn);
}
