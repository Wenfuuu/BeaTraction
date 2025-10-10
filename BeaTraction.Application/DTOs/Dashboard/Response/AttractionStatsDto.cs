namespace BeaTraction.Application.DTOs.Dashboard.Response;

public class AttractionStatsDto
{
    public Guid AttractionId { get; set; }
    public string AttractionName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int TotalRegistrations { get; set; }
    public List<ScheduleAttractionStatsDto> ScheduleAttractions { get; set; } = new();
}
