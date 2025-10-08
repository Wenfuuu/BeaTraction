using BeaTraction.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeaTraction.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Attraction> Attractions { get; set; }
    public DbSet<Registration> Registrations { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // users
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(20).IsRequired().HasDefaultValue("user");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired().HasDefaultValueSql("NOW()");
            entity.Property(e => e.RowVersion).HasColumnName("row_version").IsRequired().HasDefaultValue(1L);
            
            entity.HasIndex(e => e.Email).IsUnique();
        });
        
        // schedules
        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.ToTable("schedules");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.StartTime).HasColumnName("start_time").IsRequired();
            entity.Property(e => e.EndTime).HasColumnName("end_time").IsRequired();
            entity.Property(e => e.RowVersion).HasColumnName("row_version").IsRequired().HasDefaultValue(1L);
        });
        
        // attractions
        modelBuilder.Entity<Attraction>(entity =>
        {
            entity.ToTable("attractions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id").IsRequired();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").IsRequired();
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.Capacity).HasColumnName("capacity").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired().HasDefaultValueSql("NOW()");
            entity.Property(e => e.RowVersion).HasColumnName("row_version").IsRequired().HasDefaultValue(1L);
            
            entity.HasOne(e => e.Schedule)
                .WithMany(s => s.Attractions)
                .HasForeignKey(e => e.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // registrations
        modelBuilder.Entity<Registration>(entity =>
        {
            entity.ToTable("registrations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.AttractionId).HasColumnName("attraction_id").IsRequired();
            entity.Property(e => e.RegisteredAt).HasColumnName("registered_at").IsRequired().HasDefaultValueSql("NOW()");
            entity.Property(e => e.RowVersion).HasColumnName("row_version").IsRequired().HasDefaultValue(1L);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Registrations)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Attraction)
                .WithMany(a => a.Registrations)
                .HasForeignKey(e => e.AttractionId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => new { e.UserId, e.AttractionId })
                .IsUnique()
                .HasDatabaseName("uq_user_attraction");
        });
    }
}
