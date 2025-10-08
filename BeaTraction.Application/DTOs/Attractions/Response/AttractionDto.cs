namespace BeaTraction.Application.DTOs.Attractions.Response;

public class AttractionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int Capacity { get; set; }
    public DateTime CreatedAt { get; set; }
}
