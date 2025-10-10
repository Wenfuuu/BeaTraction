using BeaTraction.Application.Interfaces;
using BeaTraction.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BeaTraction.Infrastructure.Services;

public class SignalRNotificationService : IRealtimeNotificationService
{
    private readonly IHubContext<AttractionsHub> _hubContext;

    public SignalRNotificationService(IHubContext<AttractionsHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyRegistrationCreatedAsync(
        Guid registrationId,
        Guid userId,
        Guid scheduleAttractionId,
        DateTime registeredAt,
        DateTime occurredOn)
    {
        await _hubContext.Clients.All.SendAsync(
            "RegistrationCreated",
            new
            {
                RegistrationId = registrationId,
                UserId = userId,
                ScheduleAttractionId = scheduleAttractionId,
                RegisteredAt = registeredAt,
                OccurredOn = occurredOn
            });

        Console.WriteLine($"SignalR: Broadcast RegistrationCreated - {registrationId}");
    }

    public async Task NotifyRegistrationDeletedAsync(
        Guid registrationId,
        Guid userId,
        Guid scheduleAttractionId,
        DateTime occurredOn)
    {
        await _hubContext.Clients.All.SendAsync(
            "RegistrationDeleted",
            new
            {
                RegistrationId = registrationId,
                UserId = userId,
                ScheduleAttractionId = scheduleAttractionId,
                OccurredOn = occurredOn
            });

        Console.WriteLine($"SignalR: Broadcast RegistrationDeleted - {registrationId}");
    }
}
