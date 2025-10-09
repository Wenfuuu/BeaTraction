using BeaTraction.Application.DTOs.Schedules.Response;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Schedules;

public class UpdateScheduleHandler : IRequestHandler<UpdateScheduleCommand, ScheduleDto>
{
    private readonly IScheduleRepository _scheduleRepository;

    public UpdateScheduleHandler(IScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository;
    }

    public async Task<ScheduleDto> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(request.Id);
        if (schedule == null)
        {
            throw new InvalidOperationException("Schedule not found");
        }

        schedule.Name = request.Name;
        schedule.StartTime = request.StartTime;
        schedule.EndTime = request.EndTime;

        await _scheduleRepository.UpdateAsync(schedule);

        return new ScheduleDto
        {
            Id = schedule.Id,
            Name = schedule.Name,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime
        };
    }
}
