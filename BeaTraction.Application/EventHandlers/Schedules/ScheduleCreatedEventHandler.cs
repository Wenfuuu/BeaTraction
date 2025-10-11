using BeaTraction.Application.Common;
using BeaTraction.Application.Interfaces;
using BeaTraction.Domain.Events;
using MediatR;

namespace BeaTraction.Application.EventHandlers.Schedules;

public class ScheduleCreatedEventHandler : INotificationHandler<ScheduleCreatedEvent>
{
    private readonly ICacheService _cacheService;

    public ScheduleCreatedEventHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Handle(ScheduleCreatedEvent notification, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveByPrefixAsync(CacheKeys.SchedulesWithAttractions);
    }
}
