using BeaTraction.Application.DTOs.Schedules.Response;
using MediatR;

namespace BeaTraction.Application.Commands.Schedules;

public record CreateScheduleCommand(
    string Name,
    DateTime StartTime,
    DateTime EndTime
) : IRequest<ScheduleDto>;
