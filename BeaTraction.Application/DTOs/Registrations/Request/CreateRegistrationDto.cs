namespace BeaTraction.Application.DTOs.Registrations.Request;

public class CreateRegistrationDto
{
    public Guid UserId { get; set; }
    public Guid ScheduleAttractionId { get; set; }
    public DateTime RegisteredAt { get; set; }
}