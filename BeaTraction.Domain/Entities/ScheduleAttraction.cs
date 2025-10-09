using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeaTraction.Domain.Entities;

public class ScheduleAttraction
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Required]
    [Column("schedule_id")]
    public Guid ScheduleId { get; set; }
    
    [Required]
    [Column("attraction_id")]
    public Guid AttractionId { get; set; }
    
    [ConcurrencyCheck]
    [Column("row_version")]
    public long RowVersion { get; set; }
    
    [ForeignKey("ScheduleId")]
    public Schedule Schedule { get; set; } = null!;
    
    [ForeignKey("AttractionId")]
    public Attraction Attraction { get; set; } = null!;
    
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
