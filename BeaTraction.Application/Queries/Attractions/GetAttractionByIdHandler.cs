using BeaTraction.Application.DTOs.Attractions.Response;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Queries.Attractions;

public class GetAttractionByIdHandler : IRequestHandler<GetAttractionByIdQuery, AttractionDto>
{
    private readonly IAttractionRepository _attractionRepository;

    public GetAttractionByIdHandler(IAttractionRepository attractionRepository)
    {
        _attractionRepository = attractionRepository;
    }

    public async Task<AttractionDto> Handle(GetAttractionByIdQuery request, CancellationToken cancellationToken)
    {
        var attraction = await _attractionRepository.GetByIdAsync(request.Id);
        
        if (attraction == null)
        {
            throw new InvalidOperationException("Attraction not found");
        }

        return new AttractionDto
        {
            Id = attraction.Id,
            Name = attraction.Name,
            Description = attraction.Description,
            ImageUrl = attraction.ImageUrl,
            Capacity = attraction.Capacity,
            CreatedAt = attraction.CreatedAt
        };
    }
}
