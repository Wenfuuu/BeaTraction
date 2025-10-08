using MediatR;

namespace BeaTraction.Application.Commands.Registrations;

public record DeleteRegistrationCommand(Guid Id) : IRequest<bool>;
