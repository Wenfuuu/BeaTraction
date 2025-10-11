using BeaTraction.Application.Common;
using BeaTraction.Application.Interfaces;
using BeaTraction.Domain.Events;
using MediatR;

namespace BeaTraction.Application.EventHandlers.ScheduleAttractions;

public class ScheduleAttractionCreatedEventHandler : INotificationHandler<ScheduleAttractionCreatedEvent>
{
    private readonly ICacheService _cacheService;

    public ScheduleAttractionCreatedEventHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Handle(ScheduleAttractionCreatedEvent notification, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveByPrefixAsync(CacheKeys.AttractionStats);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.UserAttractionsPrefix);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.SchedulesWithAttractions);
    }
}
