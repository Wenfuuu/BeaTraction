using BeaTraction.Application.DTOs.Attractions.Response;
using BeaTraction.Application.Interfaces;
using BeaTraction.Domain.Entities;
using BeaTraction.Domain.Events.Attractions;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Attractions;

public class CreateAttractionHandler : IRequestHandler<CreateAttractionCommand, AttractionDto>
{
    private readonly IAttractionRepository _attractionRepository;
    private readonly IMinioService _minioService;
    private readonly IPublisher _publisher;

    public CreateAttractionHandler(
        IAttractionRepository attractionRepository, 
        IMinioService minioService,
        IPublisher publisher)
    {
        _attractionRepository = attractionRepository;
        _minioService = minioService;
        _publisher = publisher;
    }

    public async Task<AttractionDto> Handle(CreateAttractionCommand request, CancellationToken cancellationToken)
    {
        string? imageUrl = null;

        await using var stream = request.Image?.OpenReadStream();
        if (stream != null && request.Image?.Length > 0)
        {
            var fileName = await _minioService.UploadFileAsync(stream, request.Image.FileName, request.Image.ContentType);
            imageUrl = await _minioService.GetFileUrlAsync(fileName);
        }

        var attraction = new Attraction
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            ImageUrl = imageUrl,
            Capacity = request.Capacity,
            CreatedAt = DateTime.UtcNow
        };

        await _attractionRepository.AddAsync(attraction, cancellationToken);

        var domainEvent = new AttractionCreatedEvent(
            attraction.Id,
            attraction.Name,
            attraction.Capacity
        );
        await _publisher.Publish(domainEvent, cancellationToken);

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
