using BeaTraction.Application.DTOs.Attractions.Response;
using BeaTraction.Application.DTOs.ScheduleAttractions.Response;
using BeaTraction.Application.DTOs.Schedules.Response;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Queries.ScheduleAttractions;

public class GetScheduleAttractionByIdHandler : IRequestHandler<GetScheduleAttractionByIdQuery, ScheduleAttractionDto>
{
    private readonly IScheduleAttractionRepository _scheduleAttractionRepository;

    public GetScheduleAttractionByIdHandler(IScheduleAttractionRepository scheduleAttractionRepository)
    {
        _scheduleAttractionRepository = scheduleAttractionRepository;
    }

    public async Task<ScheduleAttractionDto> Handle(GetScheduleAttractionByIdQuery request, CancellationToken cancellationToken)
    {
        var scheduleAttraction = await _scheduleAttractionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (scheduleAttraction == null)
        {
            throw new Exception($"ScheduleAttraction with ID {request.Id} not found");
        }

        return new ScheduleAttractionDto
        {
            Id = scheduleAttraction.Id,
            ScheduleId = scheduleAttraction.ScheduleId,
            AttractionId = scheduleAttraction.AttractionId,
            RowVersion = scheduleAttraction.RowVersion,
            Schedule = new ScheduleDto
            {
                Id = scheduleAttraction.Schedule.Id,
                Name = scheduleAttraction.Schedule.Name,
                StartTime = scheduleAttraction.Schedule.StartTime,
                EndTime = scheduleAttraction.Schedule.EndTime
            },
            Attraction = new AttractionDto
            {
                Id = scheduleAttraction.Attraction.Id,
                Name = scheduleAttraction.Attraction.Name,
                Description = scheduleAttraction.Attraction.Description,
                ImageUrl = scheduleAttraction.Attraction.ImageUrl,
                Capacity = scheduleAttraction.Attraction.Capacity,
                CreatedAt = scheduleAttraction.Attraction.CreatedAt
            }
        };
    }
}
