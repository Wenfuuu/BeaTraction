using BeaTraction.Application.Common;
using BeaTraction.Application.Interfaces;
using BeaTraction.Domain.Events;
using MediatR;

namespace BeaTraction.Application.EventHandlers.ScheduleAttractions;

public class ScheduleAttractionDeletedEventHandler : INotificationHandler<ScheduleAttractionDeletedEvent>
{
    private readonly ICacheService _cacheService;

    public ScheduleAttractionDeletedEventHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Handle(ScheduleAttractionDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveByPrefixAsync(CacheKeys.AttractionStats);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.UserAttractionsPrefix);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.SchedulesWithAttractions);
    }
}
