using BeaTraction.Application.Common;
using BeaTraction.Application.DTOs.Attractions.Response;
using BeaTraction.Application.Interfaces;
using BeaTraction.Domain.Events.Attractions;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Attractions;

public class UpdateAttractionHandler : IRequestHandler<UpdateAttractionCommand, AttractionDto>
{
    private readonly IAttractionRepository _attractionRepository;
    private readonly IScheduleAttractionRepository _scheduleAttractionRepository;
    private readonly IMinioService _minioService;
    private readonly ICacheService _cacheService;
    private readonly IPublisher _publisher;

    public UpdateAttractionHandler(
        IAttractionRepository attractionRepository,
        IScheduleAttractionRepository scheduleAttractionRepository,
        IMinioService minioService,
        ICacheService cacheService,
        IPublisher publisher)
    {
        _attractionRepository = attractionRepository;
        _scheduleAttractionRepository = scheduleAttractionRepository;
        _minioService = minioService;
        _cacheService = cacheService;
        _publisher = publisher;
    }

    public async Task<AttractionDto> Handle(UpdateAttractionCommand request, CancellationToken cancellationToken)
    {
        var attraction = await _attractionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (attraction == null)
        {
            throw new InvalidOperationException("Attraction not found");
        }

        var oldCapacity = attraction.Capacity;

        if (request.Image != null && request.Image.Length > 0)
        {
            if (!string.IsNullOrEmpty(attraction.ImageUrl))
            {
                var oldFileName = attraction.ImageUrl.Split('/').Last();
                await _minioService.DeleteFileAsync(oldFileName);
            }

            await using var stream = request.Image.OpenReadStream();
            var fileName = await _minioService.UploadFileAsync(stream, request.Image.FileName, request.Image.ContentType);
            attraction.ImageUrl = await _minioService.GetFileUrlAsync(fileName);
        }

        attraction.Name = request.Name;
        attraction.Description = request.Description;
        attraction.Capacity = request.Capacity;

        await _attractionRepository.UpdateAsync(attraction, cancellationToken);

        // reset redis counter if capacity changed
        if (oldCapacity != request.Capacity)
        {
            var scheduleAttractions = await _scheduleAttractionRepository.GetAllAsync(cancellationToken);
            var affectedScheduleAttractions = scheduleAttractions
                .Where(sa => sa.AttractionId == request.Id)
                .ToList();

            foreach (var sa in affectedScheduleAttractions)
            {
                var capacityKey = CacheKeys.GetCapacity(sa.Id);
                var currentCount = sa.Registrations?.Count ?? 0;
                
                await _cacheService.SetStringAsync(capacityKey, currentCount.ToString(), TimeSpan.FromHours(24));
            }
        }

        var domainEvent = new AttractionUpdatedEvent(
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
