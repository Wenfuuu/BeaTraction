using BeaTraction.Application.DTOs.ScheduleAttractions.Request;
using BeaTraction.Application.DTOs.ScheduleAttractions.Response;
using MediatR;

namespace BeaTraction.Application.Commands.ScheduleAttractions;

public class CreateScheduleAttractionCommand : IRequest<ScheduleAttractionDto>
{
    public CreateScheduleAttractionDto Data { get; set; } = null!;
}
