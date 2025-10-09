namespace BeaTraction.Application.DTOs.Schedules.Request;

public class UpdateScheduleDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
