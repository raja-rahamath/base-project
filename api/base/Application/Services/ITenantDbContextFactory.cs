namespace api.Application.Services
{
    public interface ITenantDbContextFactory
    {
        api.Infrastructure.Data.ApplicationDbContext CreateDbContextForClient(string databaseName);
    }
}