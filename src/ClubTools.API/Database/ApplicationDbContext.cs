using ClubTools.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClubTools.Api.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {            
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(builder =>
            builder.OwnsOne(a => a.Tags, tagsBuilder => tagsBuilder.ToJson()));

        modelBuilder.Entity<Activity>(builder => {
            builder.OwnsOne(a => a.StepVariations, stepsBuilder => stepsBuilder.ToJson());
            builder.OwnsOne(a => a.Equipment, equipmentBuilder => equipmentBuilder.ToJson());
            });
    }

    public DbSet<Article> Articles { get; set; }

    public DbSet<Activity> Activites { get; set; }

    public DbSet<ClubEvent> ClubEvents { get; set; }
}
