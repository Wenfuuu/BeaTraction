using BeaTraction.Application.DTOs.Schedules.Response;
using MediatR;

namespace BeaTraction.Application.Commands.Schedules;

public record CreateScheduleCommand(
    Guid AttractionId,
    string Name,
    DateTime StartTime,
    DateTime EndTime
) : IRequest<ScheduleDto>;
