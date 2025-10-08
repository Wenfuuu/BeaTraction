using BeaTraction.Application.DTOs.Attractions.Response;
using MediatR;

namespace BeaTraction.Application.Commands.Attractions;

public record UpdateAttractionCommand(
    Guid Id,
    string Name,
    string Description,
    string? ImageUrl,
    int Capacity
) : IRequest<AttractionDto>;
