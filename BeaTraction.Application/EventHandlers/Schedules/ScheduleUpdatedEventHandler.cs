using BeaTraction.Application.Common;
using BeaTraction.Application.Interfaces;
using BeaTraction.Domain.Events.Schedules;
using MediatR;

namespace BeaTraction.Application.EventHandlers.Schedules;

public class ScheduleUpdatedEventHandler : INotificationHandler<ScheduleUpdatedEvent>
{
    private readonly ICacheService _cacheService;

    public ScheduleUpdatedEventHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Handle(ScheduleUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveByPrefixAsync(CacheKeys.AttractionStats);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.UserAttractionsPrefix);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.SchedulesWithAttractions);
    }
}
