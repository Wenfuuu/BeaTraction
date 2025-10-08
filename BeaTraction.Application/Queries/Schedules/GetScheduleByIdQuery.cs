using BeaTraction.Application.DTOs.Schedules.Response;
using MediatR;

namespace BeaTraction.Application.Queries.Schedules;

public record GetScheduleByIdQuery(Guid Id) : IRequest<ScheduleDto>;
