using MediatR;

namespace BeaTraction.Application.Commands.ScheduleAttractions;

public class DeleteScheduleAttractionCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
