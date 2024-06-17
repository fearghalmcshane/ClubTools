using ClubTools.Api.Database;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ClubTools.Api.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    private readonly IServiceScope _scope;
    protected readonly ISender Sender;
    protected readonly ApplicationDbContext DbContext;
    protected readonly IdentityDbContext IdentityDbContext;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();

        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        IdentityDbContext = _scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    }

    public void Dispose()
    {
        _scope?.Dispose();
        DbContext?.Dispose();
        IdentityDbContext?.Dispose();
    }
}

