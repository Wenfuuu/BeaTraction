using BeaTraction.Application.DTOs.Registrations.Response;
using MediatR;

namespace BeaTraction.Application.Queries.Registrations;

public record GetRegistrationsByUserIdQuery(Guid UserId) : IRequest<List<RegistrationDto>>;
