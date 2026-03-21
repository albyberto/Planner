using Planner.Clients.Core.Options;
using ZiggyCreatures.Caching.Fusion;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class Bootstrapper
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddClientsCore()
        {
            services.AddCacheOptions();
            services.AddCache();
            services.AddClients();

            return services;
        }

        private void AddCacheOptions()
        {
            services.AddOptions<CacheOptions>()
                .BindConfiguration(CacheOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
        
        private void AddCache() =>
            services.AddFusionCache()
                .WithOptions(options =>
                {
                    options.DefaultEntryOptions = new()
                    {
                        Duration = TimeSpan.FromMinutes(30)
                    };
                });
    }
}