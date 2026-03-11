using System.Text;
using Microsoft.Extensions.Options;
using MudBlazor.Services;
using Planner.Background;
using Planner.Infrastructure.Clients;
using Planner.Infrastructure.Options;
using Planner.Models;
using Planner.Options;
using ZiggyCreatures.Caching.Fusion;

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
            services.AddOptions<JiraApiOptions>()
                .BindConfiguration(JiraApiOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<JiraQueryOptions>()
                .BindConfiguration(JiraQueryOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<JiraFilterOptions>()
                .BindConfiguration(JiraFilterOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }

        public IServiceCollection AddJiraClients(IConfiguration configuration)
        {
            services.AddHttpClient<JiraReadClient>((provider, client) =>
            {
                var settings = provider.GetRequiredService<IOptions<JiraApiOptions>>().Value;

                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{settings.Email}:{settings.ApiToken}"));

                client.BaseAddress = new(settings.BaseUrl.TrimEnd('/') + "/");
                client.DefaultRequestHeaders.Authorization = new("Basic", credentials);
                client.DefaultRequestHeaders.Accept.Add(new("application/json"));
            });

            services.AddHttpClient<JiraWriteClient>((provider, client) =>
            {
                var settings = provider.GetRequiredService<IOptions<JiraApiOptions>>().Value;

                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{settings.Email}:{settings.ApiToken}"));

                client.BaseAddress = new(settings.BaseUrl.TrimEnd('/') + "/");
                client.DefaultRequestHeaders.Authorization = new("Basic", credentials);
                client.DefaultRequestHeaders.Accept.Add(new("application/json"));
            });

            return services;
        }

        public IServiceCollection AddBackgroundServices()
        {
            services.AddHostedService<ChatBackgroundService>();
            services.AddHostedService<BackgroundServiceBase>();

            return services;
        }

        public IServiceCollection AddCache()
        {
            services.AddFusionCache()
                .WithOptions(options =>
                {
                    options.DefaultEntryOptions = new()
                    {
                        Duration = TimeSpan.FromMinutes(30)
                    };
                });

            return services;
        }
    }
}