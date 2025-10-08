using MediatR;

namespace BeaTraction.Application.Commands.Schedules;

public record DeleteScheduleCommand(Guid Id) : IRequest<bool>;
