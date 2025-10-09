namespace BeaTraction.Application.DTOs.ScheduleAttractions.Request;

public class CreateScheduleAttractionDto
{
    public Guid ScheduleId { get; set; }
    public Guid AttractionId { get; set; }
}
