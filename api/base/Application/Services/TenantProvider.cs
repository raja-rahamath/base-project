using api.Core.Entities.SaaS;
using api.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace api.Application.Services
{
    public class TenantProvider : ITenantProvider
    {
        private readonly ApplicationDbContext _masterDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantProvider(ApplicationDbContext masterDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _masterDbContext = masterDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Client?> GetCurrentClientAsync()
        {
            var host = _httpContextAccessor.HttpContext?.Request.Host.Host;
            if (string.IsNullOrEmpty(host)) return null;

            // Extract subdomain (e.g., abc from abc.localhost)
            var parts = host.Split('.');
            if (parts.Length < 2) return null;
            var clientKey = parts[0];

            // Lookup client by domain prefix
            var client = await _masterDbContext.Clients.FirstOrDefaultAsync(c => c.DomainUrl.StartsWith(clientKey));
            return client;
        }
    }

    public class TenantDbContextFactory : ITenantDbContextFactory
    {
        private readonly IConfiguration _configuration;

        public TenantDbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ApplicationDbContext CreateDbContextForClient(string databaseName)
        {
            var baseConn = _configuration.GetConnectionString("MasterConnection");
            if (string.IsNullOrEmpty(baseConn))
                throw new InvalidOperationException("MasterConnection string is not configured.");
            var connString = baseConn.Replace("database=myapp", $"database={databaseName}");
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseMySql(connString, ServerVersion.AutoDetect(connString));
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
