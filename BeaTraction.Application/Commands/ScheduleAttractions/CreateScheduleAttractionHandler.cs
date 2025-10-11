using BeaTraction.Application.DTOs.Attractions.Response;
using BeaTraction.Application.DTOs.ScheduleAttractions.Response;
using BeaTraction.Application.DTOs.Schedules.Response;
using BeaTraction.Domain.Entities;
using BeaTraction.Domain.Events.ScheduleAttractions;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.ScheduleAttractions;

public class CreateScheduleAttractionHandler : IRequestHandler<CreateScheduleAttractionCommand, ScheduleAttractionDto>
{
    private readonly IScheduleAttractionRepository _scheduleAttractionRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IAttractionRepository _attractionRepository;
    private readonly IPublisher _publisher;

    public CreateScheduleAttractionHandler(
        IScheduleAttractionRepository scheduleAttractionRepository,
        IScheduleRepository scheduleRepository,
        IAttractionRepository attractionRepository,
        IPublisher publisher)
    {
        _scheduleAttractionRepository = scheduleAttractionRepository;
        _scheduleRepository = scheduleRepository;
        _attractionRepository = attractionRepository;
        _publisher = publisher;
    }

    public async Task<ScheduleAttractionDto> Handle(CreateScheduleAttractionCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(request.Data.ScheduleId, cancellationToken);
        if (schedule == null)
        {
            throw new InvalidOperationException($"Schedule with ID {request.Data.ScheduleId} not found");
        }

        var attraction = await _attractionRepository.GetByIdAsync(request.Data.AttractionId, cancellationToken);
        if (attraction == null)
        {
            throw new InvalidOperationException($"Attraction with ID {request.Data.AttractionId} not found");
        }

        var exists = await _scheduleAttractionRepository.ExistsAsync(request.Data.ScheduleId, request.Data.AttractionId, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException($"This attraction is already assigned to this schedule");
        }

        var hasConflict = await _scheduleAttractionRepository.HasScheduleConflictAsync(
            request.Data.AttractionId,
            schedule.StartTime,
            schedule.EndTime,
            cancellationToken);

        if (hasConflict)
        {
            throw new InvalidOperationException($"The attraction '{attraction.Name}' is already scheduled during this time period. Please choose a different time slot.");
        }

        var scheduleAttraction = new ScheduleAttraction
        {
            ScheduleId = request.Data.ScheduleId,
            AttractionId = request.Data.AttractionId,
            RowVersion = 1
        };

        var created = await _scheduleAttractionRepository.CreateAsync(scheduleAttraction, cancellationToken);

        var domainEvent = new ScheduleAttractionCreatedEvent(
            created.Id,
            created.ScheduleId,
            created.AttractionId
        );
        await _publisher.Publish(domainEvent, cancellationToken);

        return new ScheduleAttractionDto
        {
            Id = created.Id,
            ScheduleId = created.ScheduleId,
            AttractionId = created.AttractionId,
            RowVersion = created.RowVersion,
            Schedule = new ScheduleDto
            {
                Id = created.Schedule.Id,
                Name = created.Schedule.Name,
                StartTime = created.Schedule.StartTime,
                EndTime = created.Schedule.EndTime,
            },
            Attraction = new AttractionDto
            {
                Id = created.Attraction.Id,
                Name = created.Attraction.Name,
                Capacity = created.Attraction.Capacity,
                CreatedAt = created.Attraction.CreatedAt,
                Description = created.Attraction.Description,
                ImageUrl = created.Attraction.ImageUrl,
            }
        };
    }
}
