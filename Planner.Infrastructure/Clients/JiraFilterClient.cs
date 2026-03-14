using System.Collections.Immutable;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Planner.Domain;
using Planner.Domain.Responses;
using Planner.Infrastructure.Options;
using ZiggyCreatures.Caching.Fusion;
using Type = System.Type;

namespace Planner.Infrastructure.Clients;

public class JiraFilterClient(HttpClient httpClient, IFusionCache cache, IOptions<CacheOptions> cacheOptions, ILogger<JiraFilterClient> logger)
{
    private readonly CacheOptions _cacheOptions = cacheOptions.Value;

    public async Task<Project?> GetProjectAsync(string projectKey, CancellationToken cancellationToken = default) =>
        await cache.GetOrSetAsync($"jira:project:{projectKey}", async ct =>
        {
            try
            {
                return await httpClient.GetFromJsonAsync<Project>($"project/{projectKey}", ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching project {Project}", projectKey);
                return null;
            }
        }, new FusionCacheEntryOptions { Duration = _cacheOptions.Projects }, cancellationToken);

    public async Task<ImmutableArray<Project>> GetProjectsAsync(uint skip, uint take, CancellationToken cancellationToken = default) =>
        await cache.GetOrSetAsync($"jira:projects:skip:{skip}:take:{take}", async ct =>
        {
            try
            {
                var uri = $"project/search?startAt={skip}&maxResults={take}";
                var response = await httpClient.GetFromJsonAsync<ProjectResponse>(uri, ct);

                return (response?.Values ?? []).DistinctBy(project => project.Id).ToImmutableArray();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error fetching paginated projects");
                throw;
            }
        }, new FusionCacheEntryOptions { Duration = _cacheOptions.Projects }, cancellationToken);

    public async Task<ImmutableArray<Domain.Type>> GetTypesAsync(string projectKey, CancellationToken cancellationToken = default) =>
        await cache.GetOrSetAsync($"jira:types:{projectKey}", async ct =>
        {
            try
            {
                var response = await httpClient.GetFromJsonAsync<Domain.Type[]>($"project/{projectKey}/statuses", ct);
                return (response ?? []).DistinctBy(type => type.Id).ToImmutableArray();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error fetching issue types and statuses for project {Project}", projectKey);
                throw;
            }
        }, new FusionCacheEntryOptions { Duration = _cacheOptions.Types }, cancellationToken);

    public async Task<ImmutableArray<User>> GetAssigneesAsync(string projectKey, CancellationToken cancellationToken = default) =>
        await cache.GetOrSetAsync($"jira:assignees:{projectKey}", async ct =>
        {
            try
            {
                var response = await httpClient.GetFromJsonAsync<User[]>($"user/assignable/search?project={projectKey}&maxResults=1000", ct);
                return (response ?? []).Where(user => !string.IsNullOrWhiteSpace(user.EmailAddress)).DistinctBy(user => user.AccountId).ToImmutableArray();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error fetching assignees for project {Project}", projectKey);
                throw;
            }
        }, new FusionCacheEntryOptions { Duration = _cacheOptions.Assignees }, cancellationToken);

    public async Task<ImmutableArray<Component>> GetComponentsAsync(string projectKey, CancellationToken cancellationToken = default) =>
        await cache.GetOrSetAsync($"jira:components:{projectKey}", async ct =>
        {
            try
            {
                var response = await httpClient.GetFromJsonAsync<Component[]>($"project/{projectKey}/components", ct);
                return (response ?? []).DistinctBy(component => component.Id).ToImmutableArray();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error fetching components for project {Project}", projectKey);
                throw;
            }
        }, new FusionCacheEntryOptions { Duration = _cacheOptions.Components }, cancellationToken);

    public async Task<ImmutableArray<string>> GetLabelsAsync(uint skip, uint take, CancellationToken cancellationToken = default) =>
        await cache.GetOrSetAsync($"jira:labels:skip:{skip}:take:{take}", async ct =>
        {
            try
            {
                var response = await httpClient.GetFromJsonAsync<LabelResponse>($"label?startAt={skip}&maxResults={take}", ct);
                return (response?.Values ?? []).ToImmutableArray();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error fetching paginated labels");
                throw;
            }
        }, new FusionCacheEntryOptions { Duration = _cacheOptions.Labels }, cancellationToken);
}