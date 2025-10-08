namespace BeaTraction.Application.DTOs.Registrations.Response;

public class RegistrationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ScheduleId { get; set; }
    public DateTime RegisteredAt { get; set; }
}