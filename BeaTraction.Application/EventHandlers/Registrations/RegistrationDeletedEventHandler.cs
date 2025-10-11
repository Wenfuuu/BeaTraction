using BeaTraction.Application.Common;
using BeaTraction.Application.Interfaces;
using BeaTraction.Domain.Events;
using MediatR;

namespace BeaTraction.Application.EventHandlers.Registrations;

public class RegistrationDeletedEventHandler : INotificationHandler<RegistrationDeletedEvent>
{
    private readonly ICacheService _cacheService;
    private readonly IRealtimeNotificationService _realtimeNotificationService;

    public RegistrationDeletedEventHandler(
        ICacheService cacheService,
        IRealtimeNotificationService realtimeNotificationService)
    {
        _cacheService = cacheService;
        _realtimeNotificationService = realtimeNotificationService;
    }

    public async Task Handle(RegistrationDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveByPrefixAsync(CacheKeys.AttractionStats);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.GetUserAttractions(notification.UserId));
        await _cacheService.RemoveByPrefixAsync(CacheKeys.SchedulesWithAttractions);

        await _realtimeNotificationService.NotifyRegistrationDeletedAsync(
            notification.RegistrationId,
            notification.UserId,
            notification.ScheduleAttractionId,
            notification.OccurredOn);

        Console.WriteLine($"Registration deleted event handled: {notification.RegistrationId}");
    }
}
