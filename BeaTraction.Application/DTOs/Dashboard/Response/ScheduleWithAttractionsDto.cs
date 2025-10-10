namespace BeaTraction.Application.DTOs.Dashboard.Response;

public class ScheduleWithAttractionsDto
{
    public Guid ScheduleId { get; set; }
    public string ScheduleName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public long RowVersion { get; set; }
    public List<AttractionInfoDto> Attractions { get; set; } = new();
}
