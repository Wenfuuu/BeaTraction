using MediatR;

namespace BeaTraction.Application.Commands.Attractions;

public record DeleteAttractionCommand(Guid Id) : IRequest<bool>;
