using BeaTraction.Application.DTOs.Schedules.Response;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Schedules;

public class UpdateScheduleHandler : IRequestHandler<UpdateScheduleCommand, ScheduleDto>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IAttractionRepository _attractionRepository;

    public UpdateScheduleHandler(
        IScheduleRepository scheduleRepository,
        IAttractionRepository attractionRepository)
    {
        _scheduleRepository = scheduleRepository;
        _attractionRepository = attractionRepository;
    }

    public async Task<ScheduleDto> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(request.Id);
        if (schedule == null)
        {
            throw new InvalidOperationException("Schedule not found");
        }

        var attractionExists = await _attractionRepository.GetByIdAsync(request.AttractionId);
        if (attractionExists == null)
        {
            throw new InvalidOperationException("Attraction not found");
        }

        schedule.AttractionId = request.AttractionId;
        schedule.Name = request.Name;
        schedule.StartTime = request.StartTime;
        schedule.EndTime = request.EndTime;

        await _scheduleRepository.UpdateAsync(schedule);

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
