using BeaTraction.Application.DTOs.Schedules.Response;
using BeaTraction.Domain.Entities;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Schedules;

public class CreateScheduleHandler : IRequestHandler<CreateScheduleCommand, ScheduleDto>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IAttractionRepository _attractionRepository;

    public CreateScheduleHandler(
        IScheduleRepository scheduleRepository,
        IAttractionRepository attractionRepository)
    {
        _scheduleRepository = scheduleRepository;
        _attractionRepository = attractionRepository;
    }

    public async Task<ScheduleDto> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        var attractionExists = await _attractionRepository.GetByIdAsync(request.AttractionId);
        if (attractionExists == null)
        {
            throw new InvalidOperationException("Attraction not found");
        }

        var schedule = new Schedule
        {
            Id = Guid.NewGuid(),
            AttractionId = request.AttractionId,
            Name = request.Name,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        await _scheduleRepository.AddAsync(schedule);

        return new ScheduleDto
        {
            Id = schedule.Id,
            AttractionId = schedule.AttractionId,
            Name = schedule.Name,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime
        };
    }
}
