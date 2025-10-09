using BeaTraction.Application.DTOs.ScheduleAttractions.Response;
using MediatR;

namespace BeaTraction.Application.Queries.ScheduleAttractions;

public class GetAllScheduleAttractionsQuery : IRequest<IEnumerable<ScheduleAttractionDto>>
{
}
