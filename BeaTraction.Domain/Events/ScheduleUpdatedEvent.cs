namespace BeaTraction.Domain.Events;

public class ScheduleUpdatedEvent : IDomainEvent
{
    public Guid ScheduleId { get; }
    public string Name { get; }
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }
    public DateTime OccurredOn { get; }

    public ScheduleUpdatedEvent(
        Guid scheduleId,
        string name,
        DateTime startTime,
        DateTime endTime)
    {
        ScheduleId = scheduleId;
        Name = name;
        StartTime = startTime;
        EndTime = endTime;
        OccurredOn = DateTime.UtcNow;
    }
}
