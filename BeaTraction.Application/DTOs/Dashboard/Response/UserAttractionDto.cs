namespace BeaTraction.Application.DTOs.Dashboard.Response;

public class UserAttractionDto
{
    public Guid AttractionId { get; set; }
    public string AttractionName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int Capacity { get; set; }
    public List<UserScheduleAttractionDto> ScheduleAttractions { get; set; } = new();
}
