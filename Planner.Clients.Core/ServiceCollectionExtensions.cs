using System.Reflection;
using Planner.Clients.Core;
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
            services.AddOptions<CacheOptions>().BindConfiguration(CacheOptions.SectionName).ValidateDataAnnotations().ValidateOnStart();

            services.AddFusionCache().WithOptions(options =>
            {
                options.DefaultEntryOptions = new()
                {
                    Duration = TimeSpan.FromMinutes(30)
                };
            });

            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(classes => classes.InExactNamespaceOf<ProjectService>())
                .AsSelfWithInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
}