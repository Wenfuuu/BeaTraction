namespace BeaTraction.Application.DTOs.Registrations.Request;

public class CreateRegistrationDto
{
    public Guid UserId { get; set; }
    public Guid ScheduleId { get; set; }
    public DateTime RegisteredAt { get; set; }
}