using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.ScheduleAttractions;

public class DeleteScheduleAttractionHandler : IRequestHandler<DeleteScheduleAttractionCommand, bool>
{
    private readonly IScheduleAttractionRepository _scheduleAttractionRepository;

    public DeleteScheduleAttractionHandler(IScheduleAttractionRepository scheduleAttractionRepository)
    {
        _scheduleAttractionRepository = scheduleAttractionRepository;
    }

    public async Task<bool> Handle(DeleteScheduleAttractionCommand request, CancellationToken cancellationToken)
    {
        var scheduleAttraction = await _scheduleAttractionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (scheduleAttraction == null)
        {
            throw new InvalidOperationException("Schedule Attraction not found");
        }

        await _scheduleAttractionRepository.DeleteAsync(scheduleAttraction, cancellationToken);
        return true;
    }
}
