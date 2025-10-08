using BeaTraction.Application.DTOs.Attractions.Response;
using BeaTraction.Domain.Entities;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Attractions;

public class CreateAttractionHandler : IRequestHandler<CreateAttractionCommand, AttractionDto>
{
    private readonly IAttractionRepository _attractionRepository;

    public CreateAttractionHandler(IAttractionRepository attractionRepository)
    {
        _attractionRepository = attractionRepository;
    }

    public async Task<AttractionDto> Handle(CreateAttractionCommand request, CancellationToken cancellationToken)
    {
        var attraction = new Attraction
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            Capacity = request.Capacity,
            CreatedAt = DateTime.UtcNow
        };

        await _attractionRepository.AddAsync(attraction, cancellationToken);

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
