using System.Text.Json;
using Microsoft.Extensions.Options;
using Planner.Clients;
using Planner.Clients.Domain.Converters;
using Planner.Clients.Handlers;
using Planner.Clients.Options;
using Polly;
using Polly.Extensions.Http;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class Bootstrapper
{
    private static void ConfigureJiraClient(IServiceProvider provider, HttpClient client)
    {
        var settings = provider.GetRequiredService<IOptions<JiraApiOptions>>().Value;

        client.BaseAddress = new($"{settings.BaseUrl.TrimEnd('/')}/");
        client.DefaultRequestHeaders.Accept.Add(new("application/json"));
    }

    private static AsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
        HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    private static AsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() =>
        HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

    extension(IServiceCollection services)
    {
        public IServiceCollection AddClients()
        {
            services.AddOptions<JiraApiOptions>().BindConfiguration(JiraApiOptions.SectionName).ValidateDataAnnotations().ValidateOnStart();
            
            services.Configure<JsonSerializerOptions>("JiraSerializer", options =>
            {
                options.PropertyNameCaseInsensitive = true;
                options.Converters.Add(new JiraDateOnlyConverter());
                options.Converters.Add(new JiraDateTimeConverter());
            });
            
            // Register the DelegatingHandler as a Transient service
            services.AddTransient<JiraAuthenticationHandler>();

            // Register Filter Client
            services.AddHttpClient<JiraProjectClient>(ConfigureJiraClient).AddHttpMessageHandler<JiraAuthenticationHandler>().AddPolicyHandler(GetRetryPolicy()).AddPolicyHandler(GetCircuitBreakerPolicy());

            // Register Read Client
            services.AddHttpClient<JiraReadClient>(ConfigureJiraClient).AddHttpMessageHandler<JiraAuthenticationHandler>().AddPolicyHandler(GetRetryPolicy()).AddPolicyHandler(GetCircuitBreakerPolicy());

            // Register Write Client
            services.AddHttpClient<JiraWriteClient>(ConfigureJiraClient)
                .AddHttpMessageHandler<JiraAuthenticationHandler>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());

            return services;
        }
    }
}