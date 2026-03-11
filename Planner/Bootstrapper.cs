using MudBlazor.Services;
using Planner.Background;
using Planner.Options;

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

        public IServiceCollection AddBackgroundServices()
        {
            // services.AddHostedService<ChatBackgroundService>();
            // services.AddHostedService<BackgroundServiceBase>();

            return services;
        }
    }
}