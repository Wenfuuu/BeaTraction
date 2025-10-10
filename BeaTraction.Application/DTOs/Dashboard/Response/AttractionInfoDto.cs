namespace BeaTraction.Application.DTOs.Dashboard.Response;

public class AttractionInfoDto
{
    public Guid ScheduleAttractionId { get; set; }
    public Guid AttractionId { get; set; }
    public string AttractionName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int Capacity { get; set; }
}