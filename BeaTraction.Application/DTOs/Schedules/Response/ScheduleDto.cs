namespace BeaTraction.Application.DTOs.Schedules.Response;

public class ScheduleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
