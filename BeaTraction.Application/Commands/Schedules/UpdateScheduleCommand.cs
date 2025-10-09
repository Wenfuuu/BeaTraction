using BeaTraction.Application.DTOs.Schedules.Response;
using MediatR;

namespace BeaTraction.Application.Commands.Schedules;

public record UpdateScheduleCommand(
    Guid Id,
    string Name,
    DateTime StartTime,
    DateTime EndTime
) : IRequest<ScheduleDto>;
