namespace BeaTraction.Application.DTOs.Dashboard.Response;

public class UserScheduleAttractionDto
{
    public Guid ScheduleAttractionId { get; set; }
    public Guid ScheduleId { get; set; }
    public string ScheduleName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int RegistrationCount { get; set; }
    public bool IsRegistered { get; set; }
}
