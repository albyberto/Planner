using Microsoft.Extensions.Options;
using Planner.Infrastructure.Clients;
using Planner.Infrastructure.Handlers;
using Planner.Infrastructure.Options;
using Polly;
using Polly.Extensions.Http;
using ZiggyCreatures.Caching.Fusion;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Bootstrapper for infrastructure dependencies.
/// </summary>
public static class Bootstrapper
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure()
        {
            services.AddInfrastructureOptions();
            services.AddJiraClients();
            services.AddCache();

            return services;
        }

        private void AddInfrastructureOptions()
        {
            services.AddOptions<JiraApiOptions>()
                .BindConfiguration(JiraApiOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<CacheOptions>()
                .BindConfiguration(CacheOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        /// <summary>
        ///     Registers Jira HTTP clients, authentication handlers, and attaches Polly resilience policies.
        /// </summary>
        private void AddJiraClients()
        {
            // Register the DelegatingHandler as a Transient service
            services.AddTransient<JiraAuthenticationHandler>();
            
            // Register Read Client
            services.AddHttpClient<JiraReadClient>(ConfigureJiraClient)
                .AddHttpMessageHandler<JiraAuthenticationHandler>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());

            // Register Write Client
            // services.AddHttpClient<JiraWriteClient>(ConfigureJiraClient)
            //     .AddHttpMessageHandler<JiraAuthenticationHandler>()
            //     .AddPolicyHandler(GetRetryPolicy())
            //     .AddPolicyHandler(GetCircuitBreakerPolicy());
            
            // Register Filter Client
            services.AddHttpClient<JiraFilterClient>(ConfigureJiraClient)
                .AddHttpMessageHandler<JiraAuthenticationHandler>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());
        }
        
        private void AddCache() =>
            services.AddFusionCache()
                .WithOptions(options =>
                {
                    options.DefaultEntryOptions = new()
                    {
                        Duration = TimeSpan.FromMinutes(30)
                    };
                });
    }
    
    /// <summary>
    ///     Configures the base address and default headers (excluding authentication) for Jira clients.
    /// </summary>
    private static void ConfigureJiraClient(IServiceProvider provider, HttpClient client)
    {
        var settings = provider.GetRequiredService<IOptions<JiraApiOptions>>().Value;

        // Only set the Base Address and generic Accept headers here.
        // Authentication is now handled automatically by JiraAuthenticationHandler.
        client.BaseAddress = new($"{settings.BaseUrl.TrimEnd('/')}/");
        client.DefaultRequestHeaders.Accept.Add(new("application/json"));
    }

    /// <summary>
    ///     Creates a transient fault handling retry policy with exponential backoff.
    ///     Retries 3 times, waiting 2, 4, and 8 seconds.
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    /// <summary>
    ///     Creates a circuit breaker policy.
    ///     Breaks the circuit for 30 seconds after 5 consecutive transient faults.
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

}