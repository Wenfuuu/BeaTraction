using BeaTraction.Domain.Events.Attractions;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Attractions;

public class DeleteAttractionHandler : IRequestHandler<DeleteAttractionCommand, bool>
{
    private readonly IAttractionRepository _attractionRepository;
    private readonly IPublisher _publisher;

    public DeleteAttractionHandler(
        IAttractionRepository attractionRepository,
        IPublisher publisher)
    {
        _attractionRepository = attractionRepository;
        _publisher = publisher;
    }

    public async Task<bool> Handle(DeleteAttractionCommand request, CancellationToken cancellationToken)
    {
        var attraction = await _attractionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (attraction == null)
        {
            throw new InvalidOperationException("Attraction not found");
        }

        await _attractionRepository.DeleteAsync(attraction, cancellationToken);

        var domainEvent = new AttractionDeletedEvent(attraction.Id);
        await _publisher.Publish(domainEvent, cancellationToken);

        return true;
    }
}
