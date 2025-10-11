using BeaTraction.Application.Common;
using BeaTraction.Application.Interfaces;
using BeaTraction.Domain.Events.Attractions;
using MediatR;

namespace BeaTraction.Application.EventHandlers.Attractions;

public class AttractionUpdatedEventHandler : INotificationHandler<AttractionUpdatedEvent>
{
    private readonly ICacheService _cacheService;

    public AttractionUpdatedEventHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Handle(AttractionUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveByPrefixAsync(CacheKeys.AttractionStats);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.UserAttractionsPrefix);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.SchedulesWithAttractions);
    }
}
