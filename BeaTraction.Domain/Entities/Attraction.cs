using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeaTraction.Domain.Entities;

public class Attraction
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Required]
    [Column("schedule_id")]
    public Guid ScheduleId { get; set; }
    
    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [Column("description")]
    public string Description { get; set; } = string.Empty;
    
    [Column("image_url")]
    public string? ImageUrl { get; set; }
    
    [Required]
    [Column("capacity")]
    public int Capacity { get; set; }
    
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [ConcurrencyCheck]
    [Column("row_version")]
    public long RowVersion { get; set; }
    
    [ForeignKey("ScheduleId")]
    public Schedule Schedule { get; set; } = null!;
    
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
