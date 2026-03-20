using Microsoft.Extensions.DependencyInjection;
using Planner.Infrastructure.Domain.Abstract;
using Planner.Infrastructure.Repositories;

namespace Planner.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IPresetRepository>(_ => new PresetRepository($"{connectionString}.json"));
        
        return services;
    }
}