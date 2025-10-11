using BeaTraction.Application.Common;
using BeaTraction.Application.Interfaces;
using BeaTraction.Domain.Events;
using MediatR;

namespace BeaTraction.Application.EventHandlers.Schedules;

public class ScheduleDeletedEventHandler : INotificationHandler<ScheduleDeletedEvent>
{
    private readonly ICacheService _cacheService;

    public ScheduleDeletedEventHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Handle(ScheduleDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveByPrefixAsync(CacheKeys.AttractionStats);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.UserAttractionsPrefix);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.SchedulesWithAttractions);
    }
}
