using BeaTraction.Application.DTOs.ScheduleAttractions.Response;
using MediatR;

namespace BeaTraction.Application.Queries.ScheduleAttractions;

public class GetScheduleAttractionByIdQuery : IRequest<ScheduleAttractionDto>
{
    public Guid Id { get; set; }
}
