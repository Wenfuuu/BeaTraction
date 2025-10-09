using BeaTraction.Application.DTOs.ScheduleAttractions.Response;
using MediatR;

namespace BeaTraction.Application.Queries.ScheduleAttractions;

public class GetScheduleAttractionsByAttractionIdQuery : IRequest<IEnumerable<ScheduleAttractionDto>>
{
    public Guid AttractionId { get; set; }
}
