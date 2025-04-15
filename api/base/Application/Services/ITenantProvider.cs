namespace api.Application.Services
{
    public interface ITenantProvider
    {
        Task<api.Core.Entities.SaaS.Client?> GetCurrentClientAsync();
    }
}