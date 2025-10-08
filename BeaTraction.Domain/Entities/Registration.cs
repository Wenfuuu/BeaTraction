using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeaTraction.Domain.Entities;

public class Registration
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }
    
    [Required]
    [Column("schedule_id")]
    public Guid ScheduleId { get; set; }
    
    [Required]
    [Column("registered_at")]
    public DateTime RegisteredAt { get; set; }
    
    [ConcurrencyCheck]
    [Column("row_version")]
    public long RowVersion { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    
    [ForeignKey("ScheduleId")]
    public Schedule Schedule { get; set; } = null!;
}
