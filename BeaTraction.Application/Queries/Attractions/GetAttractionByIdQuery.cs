using BeaTraction.Application.DTOs.Attractions.Response;
using MediatR;

namespace BeaTraction.Application.Queries.Attractions;

public record GetAttractionByIdQuery(Guid Id) : IRequest<AttractionDto>;
