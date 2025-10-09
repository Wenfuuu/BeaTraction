using BeaTraction.Application.DTOs.ScheduleAttractions.Response;
using MediatR;

namespace BeaTraction.Application.Queries.ScheduleAttractions;

public class GetScheduleAttractionsByScheduleIdQuery : IRequest<IEnumerable<ScheduleAttractionDto>>
{
    public Guid ScheduleId { get; set; }
}
