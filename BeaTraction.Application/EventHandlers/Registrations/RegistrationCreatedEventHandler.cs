using BeaTraction.Application.Common;
using BeaTraction.Application.Interfaces;
using BeaTraction.Domain.Events;
using MediatR;

namespace BeaTraction.Application.EventHandlers.Registrations;

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
        await _cacheService.RemoveByPrefixAsync(CacheKeys.AttractionStats);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.GetUserAttractions(notification.UserId));
        await _cacheService.RemoveByPrefixAsync(CacheKeys.SchedulesWithAttractions);

        await _realtimeNotificationService.NotifyRegistrationCreatedAsync(
            notification.RegistrationId,
            notification.UserId,
            notification.ScheduleAttractionId,
            notification.RegisteredAt,
            notification.OccurredOn);

        Console.WriteLine($"Registration created event handled: {notification.RegistrationId}");
    }
}
