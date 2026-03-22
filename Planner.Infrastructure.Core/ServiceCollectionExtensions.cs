using System.Reflection;
using Planner.Infrastructure.Core;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureCore(this IServiceCollection services)
    {
        services.AddInfrastructure();

        services.Scan(scan => scan.FromAssemblies(Assembly.GetExecutingAssembly()).AddClasses(classes => classes.InExactNamespaceOf<PresetService>()).AsImplementedInterfaces().WithScopedLifetime());

        return services;
    }
}