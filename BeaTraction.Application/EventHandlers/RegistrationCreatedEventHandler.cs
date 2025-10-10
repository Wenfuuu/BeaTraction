using BeaTraction.Application.Interfaces;
using BeaTraction.Domain.Events;
using MediatR;

namespace BeaTraction.Application.EventHandlers;

public class RegistrationCreatedEventHandler : INotificationHandler<RegistrationCreatedEvent>
{
    private readonly ICacheService _cacheService;
    private readonly IRealtimeNotificationService _realtimeNotificationService;

    public RegistrationCreatedEventHandler(
        ICacheService cacheService,
        IRealtimeNotificationService realtimeNotificationService)
    {
        _cacheService = cacheService;
        _realtimeNotificationService = realtimeNotificationService;
    }

    public async Task Handle(RegistrationCreatedEvent notification, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveByPrefixAsync("attraction-stats");
        await _cacheService.RemoveByPrefixAsync($"user-attractions:{notification.UserId}");
        await _cacheService.RemoveByPrefixAsync("schedules-with-attractions");

        await _realtimeNotificationService.NotifyRegistrationCreatedAsync(
            notification.RegistrationId,
            notification.UserId,
            notification.ScheduleAttractionId,
            notification.RegisteredAt,
            notification.OccurredOn);

        Console.WriteLine($"Registration created event handled: {notification.RegistrationId}");
    }
}
