using BeaTraction.Application.DTOs.Dashboard.Response;
using BeaTraction.Application.Interfaces;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Queries.Dashboard;

public class GetUserAttractionsHandler : IRequestHandler<GetUserAttractionsQuery, List<UserAttractionDto>>
{
    private readonly IAttractionRepository _attractionRepository;
    private readonly IScheduleAttractionRepository _scheduleAttractionRepository;
    private readonly IRegistrationRepository _registrationRepository;
    private readonly ICacheService _cacheService;
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

    public GetUserAttractionsHandler(
        IAttractionRepository attractionRepository,
        IScheduleAttractionRepository scheduleAttractionRepository,
        IRegistrationRepository registrationRepository,
        ICacheService cacheService)
    {
        _attractionRepository = attractionRepository;
        _scheduleAttractionRepository = scheduleAttractionRepository;
        _registrationRepository = registrationRepository;
        _cacheService = cacheService;
    }

    public async Task<List<UserAttractionDto>> Handle(GetUserAttractionsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"user-attractions:{request.UserId}";

        var cachedAttractions = await _cacheService.GetAsync<List<UserAttractionDto>>(cacheKey);
        if (cachedAttractions != null)
        {
            Console.WriteLine($"Returning user attractions from cache for user: {request.UserId}");
            return cachedAttractions;
        }

        Console.WriteLine($"Fetching user attractions from database for user: {request.UserId}");

        var attractions = await _attractionRepository.GetAllAsync(cancellationToken);
        var scheduleAttractions = await _scheduleAttractionRepository.GetAllAsync(cancellationToken);

        var userRegistrations = await _registrationRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var userRegistrationIds = userRegistrations.Select(r => r.ScheduleAttractionId).ToHashSet();

        var userAttractions = attractions
            .Select(attraction =>
            {
                var attractionSchedules = scheduleAttractions
                    .Where(sa => sa.AttractionId == attraction.Id)
                    .ToList();

                var scheduleStats = attractionSchedules.Select(sa => new UserScheduleAttractionDto
                {
                    ScheduleAttractionId = sa.Id,
                    ScheduleId = sa.ScheduleId,
                    ScheduleName = sa.Schedule?.Name ?? "Unknown Schedule",
                    StartTime = sa.Schedule?.StartTime ?? DateTime.MinValue,
                    EndTime = sa.Schedule?.EndTime ?? DateTime.MinValue,
                    RegistrationCount = sa.Registrations?.Count ?? 0,
                    IsRegistered = userRegistrationIds.Contains(sa.Id)
                }).ToList();

                return new
                {
                    Attraction = new UserAttractionDto
                    {
                        AttractionId = attraction.Id,
                        AttractionName = attraction.Name,
                        Description = attraction.Description,
                        ImageUrl = attraction.ImageUrl,
                        Capacity = attraction.Capacity,
                        ScheduleAttractions = scheduleStats
                    },
                    HasSchedules = scheduleStats.Count > 0
                };
            })
            .Where(x => x.HasSchedules)
            .Select(x => x.Attraction)
            .ToList();

        await _cacheService.SetAsync(cacheKey, userAttractions, CacheExpiration);

        return userAttractions;
    }
}
