using BeaTraction.Application.DTOs.Attractions.Response;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Attractions;

public class UpdateAttractionHandler : IRequestHandler<UpdateAttractionCommand, AttractionDto>
{
    private readonly IAttractionRepository _attractionRepository;

    public UpdateAttractionHandler(IAttractionRepository attractionRepository)
    {
        _attractionRepository = attractionRepository;
    }

    public async Task<AttractionDto> Handle(UpdateAttractionCommand request, CancellationToken cancellationToken)
    {
        var attraction = await _attractionRepository.GetByIdAsync(request.Id);
        if (attraction == null)
        {
            throw new InvalidOperationException("Attraction not found");
        }

        attraction.Name = request.Name;
        attraction.Description = request.Description;
        attraction.ImageUrl = request.ImageUrl;
        attraction.Capacity = request.Capacity;

        await _attractionRepository.UpdateAsync(attraction);

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
