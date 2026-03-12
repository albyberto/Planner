using MudBlazor.Services;
using Planner.Background;
using Planner.Options;
using Planner.Services;
using Planner.Stores;

namespace Planner;

public static class Bootstrapper
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMudBlazor()
    {
        // Blazor
        services
            .AddRazorComponents()
            .AddInteractiveServerComponents();

        // MudBlazor
        services.AddMudServices();

        return services;
    }

    public IServiceCollection AddPlannerOptions()
    {
        services.AddOptions<JiraFilterOptions>()
            .BindConfiguration(JiraFilterOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    public IServiceCollection AddStores()
    {
        services.AddSingleton<FilterStore>();
        services.AddSingleton<IssueStore>();

        return services;
    }

    public IServiceCollection AddPlannerServices()
    {
        services.AddScoped<ProjectsService>();

        return services;
    }

    public IServiceCollection AddBackgroundServices()
    {
        services.AddHostedService<DashboardBackgroundService>();

        return services;
    }
}
}