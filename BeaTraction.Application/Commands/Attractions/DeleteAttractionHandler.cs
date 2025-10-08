using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Attractions;

public class DeleteAttractionHandler : IRequestHandler<DeleteAttractionCommand, bool>
{
    private readonly IAttractionRepository _attractionRepository;

    public DeleteAttractionHandler(IAttractionRepository attractionRepository)
    {
        _attractionRepository = attractionRepository;
    }

    public async Task<bool> Handle(DeleteAttractionCommand request, CancellationToken cancellationToken)
    {
        var attraction = await _attractionRepository.GetByIdAsync(request.Id);
        if (attraction == null)
        {
            throw new InvalidOperationException("Attraction not found");
        }

        await _attractionRepository.DeleteAsync(attraction);
        return true;
    }
}
