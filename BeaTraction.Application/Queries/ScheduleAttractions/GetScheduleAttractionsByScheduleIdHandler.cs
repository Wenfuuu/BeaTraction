using BeaTraction.Application.DTOs.Attractions.Response;
using BeaTraction.Application.DTOs.ScheduleAttractions.Response;
using BeaTraction.Application.DTOs.Schedules.Response;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Queries.ScheduleAttractions;

public class GetScheduleAttractionsByScheduleIdHandler : IRequestHandler<GetScheduleAttractionsByScheduleIdQuery, IEnumerable<ScheduleAttractionDto>>
{
    private readonly IScheduleAttractionRepository _scheduleAttractionRepository;

    public GetScheduleAttractionsByScheduleIdHandler(IScheduleAttractionRepository scheduleAttractionRepository)
    {
        _scheduleAttractionRepository = scheduleAttractionRepository;
    }

    public async Task<IEnumerable<ScheduleAttractionDto>> Handle(GetScheduleAttractionsByScheduleIdQuery request, CancellationToken cancellationToken)
    {
        var scheduleAttractions = await _scheduleAttractionRepository.GetByScheduleIdAsync(request.ScheduleId, cancellationToken);

        return scheduleAttractions.Select(sa => new ScheduleAttractionDto
        {
            Id = sa.Id,
            ScheduleId = sa.ScheduleId,
            AttractionId = sa.AttractionId,
            RowVersion = sa.RowVersion,
            Schedule = new ScheduleDto
            {
                Id = sa.Schedule.Id,
                Name = sa.Schedule.Name,
                StartTime = sa.Schedule.StartTime,
                EndTime = sa.Schedule.EndTime
            },
            Attraction = new AttractionDto
            {
                Id = sa.Attraction.Id,
                Name = sa.Attraction.Name,
                Description = sa.Attraction.Description,
                ImageUrl = sa.Attraction.ImageUrl,
                Capacity = sa.Attraction.Capacity,
                CreatedAt = sa.Attraction.CreatedAt
            }
        });
    }
}
