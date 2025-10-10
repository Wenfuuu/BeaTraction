using BeaTraction.Application.DTOs.Dashboard.Response;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Queries.Dashboard;

public class GetSchedulesWithAttractionsHandler : IRequestHandler<GetSchedulesWithAttractionsQuery, List<ScheduleWithAttractionsDto>>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IScheduleAttractionRepository _scheduleAttractionRepository;

    public GetSchedulesWithAttractionsHandler(
        IScheduleRepository scheduleRepository,
        IScheduleAttractionRepository scheduleAttractionRepository)
    {
        _scheduleRepository = scheduleRepository;
        _scheduleAttractionRepository = scheduleAttractionRepository;
    }

    public async Task<List<ScheduleWithAttractionsDto>> Handle(GetSchedulesWithAttractionsQuery request, CancellationToken cancellationToken)
    {
        var schedules = await _scheduleRepository.GetAllAsync(cancellationToken);
        var scheduleAttractions = await _scheduleAttractionRepository.GetAllAsync(cancellationToken);

        var schedulesWithAttractions = schedules.Select(schedule =>
        {
            var attractions = scheduleAttractions
                .Where(sa => sa.ScheduleId == schedule.Id)
                .Select(sa => new AttractionInfoDto
                {
                    ScheduleAttractionId = sa.Id,
                    AttractionId = sa.AttractionId,
                    AttractionName = sa.Attraction?.Name ?? "Unknown",
                    Description = sa.Attraction?.Description ?? "",
                    ImageUrl = sa.Attraction?.ImageUrl,
                    Capacity = sa.Attraction?.Capacity ?? 0
                })
                .ToList();

            return new ScheduleWithAttractionsDto
            {
                ScheduleId = schedule.Id,
                ScheduleName = schedule.Name,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                RowVersion = schedule.RowVersion,
                Attractions = attractions
            };
        }).ToList();

        return schedulesWithAttractions;
    }
}
