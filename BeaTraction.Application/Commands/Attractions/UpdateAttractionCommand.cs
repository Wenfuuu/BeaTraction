using BeaTraction.Application.DTOs.Attractions.Response;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BeaTraction.Application.Commands.Attractions;

public record UpdateAttractionCommand(
    Guid Id,
    string Name,
    string Description,
    IFormFile? Image,
    int Capacity
) : IRequest<AttractionDto>;
