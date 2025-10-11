using BeaTraction.Application.Common;
using BeaTraction.Application.Interfaces;
using BeaTraction.Domain.Events.Attractions;
using MediatR;

namespace BeaTraction.Application.EventHandlers.Attractions;

public class AttractionDeletedEventHandler : INotificationHandler<AttractionDeletedEvent>
{
    private readonly ICacheService _cacheService;

    public AttractionDeletedEventHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Handle(AttractionDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveByPrefixAsync(CacheKeys.AttractionStats);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.UserAttractionsPrefix);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.SchedulesWithAttractions);
    }
}
