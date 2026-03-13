using System.Collections.Immutable;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Planner.Domain;
using Planner.Domain.Responses;
using Planner.Infrastructure.Options;
using ZiggyCreatures.Caching.Fusion;
using Type = Planner.Domain.Type;

namespace Planner.Infrastructure.Clients;

public class JiraFilterClient(HttpClient httpClient, IFusionCache cache, IOptions<CacheOptions> cacheOptions, ILogger<JiraFilterClient> logger)
{
    private readonly CacheOptions _cacheOptions = cacheOptions.Value;

    public async Task<ImmutableHashSet<Project>> GetProjectsAsync(uint skip, uint take, CancellationToken cancellationToken = default) =>
        await cache.GetOrSetAsync("jira:projects:global", async ct =>
        {
            try
            {
                var uri = $"project?startAt{skip}=&maxResults={take}";
                var response = await httpClient.GetFromJsonAsync<List<Project>>(uri, ct);
                return (response ?? []).DistinctBy(project => project.Id).ToImmutableHashSet();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error fetching global projects");
                throw;
            }
        }, new FusionCacheEntryOptions { Duration = _cacheOptions.Projects }, cancellationToken);

    public async Task<ImmutableHashSet<Type>> GetTypesAsync(string projectKey, CancellationToken cancellationToken = default) =>
        await cache.GetOrSetAsync($"jira:types:{projectKey}", async ct =>
        {
            try
            {
                var response = await httpClient.GetFromJsonAsync<List<Type>>($"project/{projectKey}/statuses", ct);
                return (response ?? []).DistinctBy(type => type.Id).ToImmutableHashSet();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error fetching issue types and statuses for project {Project}", projectKey);
                throw;
            }
        }, new FusionCacheEntryOptions { Duration = _cacheOptions.Types }, cancellationToken);

    public async Task<ImmutableHashSet<User>> GetAssigneesAsync(string projectKey, CancellationToken cancellationToken = default) =>
        await cache.GetOrSetAsync($"jira:assignees:{projectKey}", async ct =>
        {
            try
            {
                var response = await httpClient.GetFromJsonAsync<List<User>>($"user/assignable/search?project={projectKey}", ct);
                return (response ?? []).Where(user => !string.IsNullOrWhiteSpace(user.EmailAddress)).DistinctBy(user => user.AccountId).ToImmutableHashSet();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error fetching assignees for project {Project}", projectKey);
                throw;
            }
        }, new FusionCacheEntryOptions { Duration = _cacheOptions.Assignees }, cancellationToken);

    public async Task<ImmutableHashSet<Component>> GetComponentsAsync(string projectKey, CancellationToken cancellationToken = default) =>
        await cache.GetOrSetAsync($"jira:components:{projectKey}", async ct =>
        {
            try
            {
                var response = await httpClient.GetFromJsonAsync<List<Component>>($"project/{projectKey}/components", ct);
                return (response ?? []).DistinctBy(component => component.Id).ToImmutableHashSet();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error fetching components for project {Project}", projectKey);
                throw;
            }
        }, new FusionCacheEntryOptions { Duration = _cacheOptions.Components }, cancellationToken);

    public async Task<ImmutableHashSet<string>> GetLabelsAsync(uint skip, uint take, CancellationToken cancellationToken = default) =>
        await cache.GetOrSetAsync("jira:labels:global", async ct =>
        {
            try
            {
                var accumulator = new HashSet<string>();
                var startAt = 0;
                bool isLast;

                do
                {
                    var response = await httpClient.GetFromJsonAsync<LabelResponse>($"label??startAt{skip}=&maxResults={take}", ct);

                    if (response?.Values is not null)
                        foreach (var label in response.Values)
                            accumulator.Add(label);

                    isLast = response?.IsLast ?? true;

                    if (response is not null) startAt += response.MaxResults;
                } while (!isLast);

                return accumulator.ToImmutableHashSet();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error fetching global labels");
                throw;
            }
        }, new FusionCacheEntryOptions { Duration = _cacheOptions.Labels }, cancellationToken);
}