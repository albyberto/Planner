using Microsoft.Extensions.DependencyInjection;

namespace Planner.Infrastructure.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureCore(this IServiceCollection services)
    {
        services.AddSingleton<IPresetService, PresetService>();
        
        return services;
    }
}