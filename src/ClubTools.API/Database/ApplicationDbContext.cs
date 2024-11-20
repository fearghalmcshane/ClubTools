using ClubTools.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace ClubTools.Api.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {            
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(ConfigureArticle);

        modelBuilder.Entity<Activity>(ConfigureActivity);

        modelBuilder.Entity<Facility>(ConfigureFacility);
    }

    private void ConfigureActivity(EntityTypeBuilder<Activity> builder)
    {
        builder.Property(a => a.StepVariations)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<string>>(v));

        builder.Property(a => a.Equipment)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<string>>(v));
    }

    private void ConfigureArticle(EntityTypeBuilder<Article> builder)
    {
        builder.Property(a => a.Tags)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<string>>(v));
    }

    private void ConfigureFacility(EntityTypeBuilder<Facility> builder)
    {
        builder.Property(f => f.Amenities)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<string>>(v));
    }

    public DbSet<Article> Articles { get; set; }

    public DbSet<Activity> Activites { get; set; }

    public DbSet<ClubEvent> ClubEvents { get; set; }

    public DbSet<Facility> Facilities { get; set; }
}
