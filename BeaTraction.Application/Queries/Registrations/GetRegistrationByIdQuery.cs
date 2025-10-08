using BeaTraction.Application.DTOs.Registrations.Response;
using MediatR;

namespace BeaTraction.Application.Queries.Registrations;

public record GetRegistrationByIdQuery(Guid Id) : IRequest<RegistrationDto>;