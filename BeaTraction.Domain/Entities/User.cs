using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeaTraction.Domain.Entities;

public class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("role")]
    public string Role { get; set; } = "user";
    
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [ConcurrencyCheck]
    [Column("row_version")]
    public long RowVersion { get; set; }
    
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
