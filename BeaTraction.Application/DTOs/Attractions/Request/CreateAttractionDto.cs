using Microsoft.AspNetCore.Http;

namespace BeaTraction.Application.DTOs.Attractions.Request;

public class CreateAttractionDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IFormFile? Image { get; set; }
    public int Capacity { get; set; }
}
