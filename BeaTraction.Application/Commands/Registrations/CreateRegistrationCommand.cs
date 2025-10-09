using BeaTraction.Application.DTOs.Registrations.Response;
using MediatR;

namespace BeaTraction.Application.Commands.Registrations;

public record CreateRegistrationCommand(
    Guid UserId,
    Guid ScheduleAttractionId,
    DateTime RegisteredAt
) : IRequest<RegistrationDto>;