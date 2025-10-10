using BeaTraction.Application.DTOs.Dashboard.Response;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Queries.Dashboard;

public class GetAttractionStatsHandler : IRequestHandler<GetAttractionStatsQuery, List<AttractionStatsDto>>
{
    private readonly IAttractionRepository _attractionRepository;
    private readonly IScheduleAttractionRepository _scheduleAttractionRepository;

    public GetAttractionStatsHandler(
        IAttractionRepository attractionRepository,
        IScheduleAttractionRepository scheduleAttractionRepository)
    {
        _attractionRepository = attractionRepository;
        _scheduleAttractionRepository = scheduleAttractionRepository;
    }

    public async Task<List<AttractionStatsDto>> Handle(GetAttractionStatsQuery request, CancellationToken cancellationToken)
    {
        var attractions = await _attractionRepository.GetAllAsync(cancellationToken);
        var scheduleAttractions = await _scheduleAttractionRepository.GetAllAsync(cancellationToken);

        var stats = attractions.Select(attraction =>
        {
            var attractionSchedules = scheduleAttractions
                .Where(sa => sa.AttractionId == attraction.Id)
                .ToList();

            var scheduleStats = attractionSchedules.Select(sa => new ScheduleAttractionStatsDto
            {
                ScheduleAttractionId = sa.Id,
                ScheduleId = sa.ScheduleId,
                ScheduleName = sa.Schedule?.Name ?? "Unknown Schedule",
                StartTime = sa.Schedule?.StartTime ?? DateTime.MinValue,
                EndTime = sa.Schedule?.EndTime ?? DateTime.MinValue,
                RegistrationCount = sa.Registrations?.Count ?? 0
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

        return stats;
    }
}
