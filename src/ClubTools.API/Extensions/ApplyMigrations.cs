using ClubTools.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace ClubTools.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }

    public static void ApplyIdentityMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<IdentityDbContext>();

        dbContext.Database.Migrate();
    }
}
