using BeaTraction.Application.DTOs.Registrations.Response;
using MediatR;

namespace BeaTraction.Application.Commands.Registrations;

public record UpdateRegistrationCommand(
    Guid Id,
    Guid UserId,
    Guid ScheduleAttractionId,
    DateTime RegisteredAt
) : IRequest<RegistrationDto>;
