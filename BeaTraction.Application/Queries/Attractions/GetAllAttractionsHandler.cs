using BeaTraction.Application.DTOs.Attractions.Response;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Queries.Attractions;

public class GetAllAttractionsHandler : IRequestHandler<GetAllAttractionsQuery, List<AttractionDto>>
{
    private readonly IAttractionRepository _attractionRepository;

    public GetAllAttractionsHandler(IAttractionRepository attractionRepository)
    {
        _attractionRepository = attractionRepository;
    }

    public async Task<List<AttractionDto>> Handle(GetAllAttractionsQuery request, CancellationToken cancellationToken)
    {
        var attractions = await _attractionRepository.GetAllAsync();

        return attractions.Select(a => new AttractionDto
        {
            Id = a.Id,
            Name = a.Name,
            Description = a.Description,
            ImageUrl = a.ImageUrl,
            Capacity = a.Capacity,
            CreatedAt = a.CreatedAt
        }).ToList();
    }
}
