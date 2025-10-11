namespace BeaTraction.Domain.Events.Schedules;

public class ScheduleDeletedEvent : IDomainEvent
{
    public Guid ScheduleId { get; }
    public DateTime OccurredOn { get; }

    public ScheduleDeletedEvent(Guid scheduleId)
    {
        ScheduleId = scheduleId;
        OccurredOn = DateTime.UtcNow;
    }
}
