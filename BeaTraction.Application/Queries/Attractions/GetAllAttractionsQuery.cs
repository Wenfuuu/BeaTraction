using BeaTraction.Application.DTOs.Attractions.Response;
using MediatR;

namespace BeaTraction.Application.Queries.Attractions;

public record GetAllAttractionsQuery : IRequest<List<AttractionDto>>;
