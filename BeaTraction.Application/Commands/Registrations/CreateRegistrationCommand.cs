using BeaTraction.Application.DTOs.Registrations.Response;
using MediatR;

namespace BeaTraction.Application.Commands.Registrations;

public record CreateRegistrationCommand(
    Guid UserId,
    Guid ScheduleId,
    DateTime RegisteredAt
) : IRequest<RegistrationDto>;