using System.Reflection;
using MudBlazor.Services;
using Planner.Background;
using Planner.Facades;
using Planner.Options;
using Planner.Stores;

namespace Planner;

public static class Bootstrapper
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMudBlazor()
        {
            // Blazor
            services.AddRazorComponents().AddInteractiveServerComponents();

            // MudBlazor
            services.AddMudServices();

            return services;
        }

        public IServiceCollection AddPlanner()
        {
            services.AddOptions<JiraFilterOptions>().BindConfiguration(JiraFilterOptions.SectionName).ValidateDataAnnotations().ValidateOnStart();

            services.Scan(scan => scan.FromAssemblies(Assembly.GetExecutingAssembly()).AddClasses(classes => classes.InExactNamespaceOf<FilterStore>()).AsSelfWithInterfaces().WithSingletonLifetime());

            services.Scan(scan => scan.FromAssemblies(Assembly.GetExecutingAssembly()).AddClasses(classes => classes.InExactNamespaceOf<ProjectFacade>()).AsSelfWithInterfaces().WithScopedLifetime());

            return services;
        }

        public IServiceCollection AddBackgroundServices()
        {
            services.AddHostedService<DashboardBackgroundService>();

            return services;
        }
    }
}