using BeaTraction.Application.DTOs.Schedules.Response;
using MediatR;

namespace BeaTraction.Application.Queries.Schedules;

public record GetAllSchedulesQuery : IRequest<List<ScheduleDto>>;
