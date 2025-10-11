using BeaTraction.Application.Common;
using BeaTraction.Application.DTOs.Dashboard.Response;
using BeaTraction.Application.Interfaces;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Queries.Dashboard;

public class GetAttractionStatsHandler : IRequestHandler<GetAttractionStatsQuery, List<AttractionStatsDto>>
{
    private readonly IAttractionRepository _attractionRepository;
    private readonly IScheduleAttractionRepository _scheduleAttractionRepository;
    private readonly ICacheService _cacheService;
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

    public GetAttractionStatsHandler(
        IAttractionRepository attractionRepository,
        IScheduleAttractionRepository scheduleAttractionRepository,
        ICacheService cacheService)
    {
        _attractionRepository = attractionRepository;
        _scheduleAttractionRepository = scheduleAttractionRepository;
        _cacheService = cacheService;
    }

    public async Task<List<AttractionStatsDto>> Handle(GetAttractionStatsQuery request, CancellationToken cancellationToken)
    {
        var cachedStats = await _cacheService.GetAsync<List<AttractionStatsDto>>(CacheKeys.AttractionStats);
        if (cachedStats != null)
        {
            Console.WriteLine("Returning attraction stats from cache");
            return cachedStats;
        }

        Console.WriteLine("Fetching attraction stats from database");

        var attractions = await _attractionRepository.GetAllAsync(cancellationToken);
        var scheduleAttractions = await _scheduleAttractionRepository.GetAllAsync(cancellationToken);

        var stats = attractions.Select(attraction =>
        {
            var attractionSchedules = scheduleAttractions
                .Where(sa => sa.AttractionId == attraction.Id)
                .ToList();

            var scheduleStats = attractionSchedules.Select(sa =>
            {
                var dbRegistrationCount = sa.Registrations?.Count ?? 0;
                
                var capacityKey = CacheKeys.GetCapacity(sa.Id);
                var redisCountStr = _cacheService.GetStringAsync(capacityKey).Result;
                
                int registrationCount;
                if (!string.IsNullOrEmpty(redisCountStr) && int.TryParse(redisCountStr, out var redisCount))
                {
                    registrationCount = redisCount;
                }
                else
                {
                    registrationCount = dbRegistrationCount;
                    _cacheService.SetStringAsync(capacityKey, registrationCount.ToString(), TimeSpan.FromHours(24));
                }

                var capacity = attraction.Capacity;
                var availableSpots = Math.Max(0, capacity - registrationCount);

                return new ScheduleAttractionStatsDto
                {
                    ScheduleAttractionId = sa.Id,
                    ScheduleId = sa.ScheduleId,
                    ScheduleName = sa.Schedule?.Name ?? "Unknown Schedule",
                    StartTime = sa.Schedule?.StartTime ?? DateTime.MinValue,
                    EndTime = sa.Schedule?.EndTime ?? DateTime.MinValue,
                    RegistrationCount = registrationCount,
                    AvailableSpots = availableSpots,
                    IsFull = registrationCount >= capacity
                };
            }).ToList();

            return new AttractionStatsDto
            {
                AttractionId = attraction.Id,
                AttractionName = attraction.Name,
                Capacity = attraction.Capacity,
                TotalRegistrations = scheduleStats.Sum(s => s.RegistrationCount),
                ScheduleAttractions = scheduleStats
            };
        }).ToList();

        await _cacheService.SetAsync(CacheKeys.AttractionStats, stats, CacheExpiration);

        return stats;
    }
}
