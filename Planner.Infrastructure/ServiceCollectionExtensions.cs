using System.Reflection;
using Planner.Infrastructure.Data;
using Planner.Infrastructure.Repositories;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDatabaseContext, JsonDatabaseContext>();

        services.Scan(scan => scan.FromAssemblies(Assembly.GetExecutingAssembly()).AddClasses(classes => classes.InExactNamespaceOf<PresetRepository>()).AsImplementedInterfaces().WithScopedLifetime());

        return services;
    }
}