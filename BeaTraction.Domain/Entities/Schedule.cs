using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeaTraction.Domain.Entities;

public class Schedule
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [Column("start_time")]
    public DateTime StartTime { get; set; }
    
    [Required]
    [Column("end_time")]
    public DateTime EndTime { get; set; }
    
    [ConcurrencyCheck]
    [Column("row_version")]
    public long RowVersion { get; set; }
    
    public ICollection<Attraction> Attractions { get; set; } = new List<Attraction>();
}
