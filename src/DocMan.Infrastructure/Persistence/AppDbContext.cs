using DocMan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocMan.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Document> Documents => Set<Document>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(250).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.FileExtension).HasMaxLength(20).IsRequired();
            entity.Property(e => e.ContentHash).HasMaxLength(128).IsRequired();
            entity.Property(e => e.UploadedBy).HasMaxLength(100).IsRequired();

            // Index for content hash duplicate detection
            entity.HasIndex(e => e.ContentHash);

            // Index for search performance
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.UploadedBy);

            // Tags stored as JSON
            entity.Property(e => e.Tags)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
                .HasMaxLength(2000);
        });
    }
}
