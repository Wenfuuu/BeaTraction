using BeaTraction.Application.DTOs.Attractions.Response;
using BeaTraction.Application.DTOs.Schedules.Response;

namespace BeaTraction.Application.DTOs.ScheduleAttractions.Response;

public class ScheduleAttractionDto
{
    public Guid Id { get; set; }
    public Guid ScheduleId { get; set; }
    public Guid AttractionId { get; set; }
    public long RowVersion { get; set; }
    public ScheduleDto? Schedule { get; set; }
    public AttractionDto? Attraction { get; set; }
}
