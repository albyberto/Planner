using Microsoft.Extensions.DependencyInjection;
using Planner.Infrastructure.Core.Abstract;

namespace Planner.Infrastructure.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureCore(this IServiceCollection services, string connection)
    {
        services.AddSingleton<IPresetService, PresetService>();
        services.AddInfrastructure(connection);
        
        return services;
    }
}