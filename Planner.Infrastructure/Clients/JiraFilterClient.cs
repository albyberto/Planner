using System.Collections.Immutable;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Planner.Domain;
using Planner.Infrastructure.Options;

namespace Planner.Infrastructure.Clients;

/// <summary>
/// Client responsible for fetching and caching Jira filters and related metadata.
/// </summary>
public class JiraFilterClient(HttpClient httpClient, IDistributedCache cache, IOptions<CacheOptions> cacheOptions, ILogger<JiraFilterClient> logger)
{
    private readonly CacheOptions _cacheOptions = cacheOptions.Value;
    
    public async Task<ImmutableHashSet<Project>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "jira:projects:global";
        var cachedBytes = await cache.GetAsync(cacheKey, cancellationToken);

        if (cachedBytes is not null) 
            return JsonSerializer.Deserialize<ImmutableHashSet<Project>>(cachedBytes) ?? [];

        try
        {
            var response = await httpClient.GetFromJsonAsync<List<Project>>("project", cancellationToken);

            var projects = (response ?? [])
                .DistinctBy(project => project.Id)
                .ToImmutableHashSet();

            await cache.SetAsync(cacheKey, JsonSerializer.SerializeToUtf8Bytes(projects), _cacheOptions.Projects, cancellationToken);

            return projects;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error fetching global projects");
            throw;
        }
    }

    public async Task<ImmutableHashSet<IssueType>> GetTypesAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"jira:types:{projectKey}";
        var cachedBytes = await cache.GetAsync(cacheKey, cancellationToken);

        if (cachedBytes is not null) 
            return JsonSerializer.Deserialize<ImmutableHashSet<IssueType>>(cachedBytes) ?? [];

        try
        {
            var response = await httpClient.GetFromJsonAsync<List<IssueType>>($"project/{projectKey}/statuses", cancellationToken);
            
            var types = (response ?? [])
                .DistinctBy(type => type.Id)
                .ToImmutableHashSet();

            await cache.SetAsync(cacheKey, JsonSerializer.SerializeToUtf8Bytes(types), _cacheOptions.Types, cancellationToken);

            return types;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error fetching issue types and statuses for project {Project}", projectKey);
            throw;
        }
    }

    public async Task<ImmutableHashSet<User>> GetAssigneesAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"jira:assignees:{projectKey}";
        var cachedBytes = await cache.GetAsync(cacheKey, cancellationToken);

        if (cachedBytes is not null) 
            return JsonSerializer.Deserialize<ImmutableHashSet<User>>(cachedBytes) ?? [];

        try
        {
            var response = await httpClient.GetFromJsonAsync<List<User>>($"user/assignable/search?project={projectKey}", cancellationToken);
            
            var assignees = (response ?? [])
                .Where(user => !string.IsNullOrWhiteSpace(user.EmailAddress))
                .DistinctBy(user => user.AccountId)
                .ToImmutableHashSet();

            await cache.SetAsync(cacheKey, JsonSerializer.SerializeToUtf8Bytes(assignees), _cacheOptions.Assignees, cancellationToken);

            return assignees;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error fetching assignees for project {Project}", projectKey);
            throw;
        }
    }

    public async Task<ImmutableHashSet<Component>> GetComponentsAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"jira:components:{projectKey}";
        var cachedBytes = await cache.GetAsync(cacheKey, cancellationToken);

        if (cachedBytes is not null) 
            return JsonSerializer.Deserialize<ImmutableHashSet<Component>>(cachedBytes) ?? [];

        try
        {
            var response = await httpClient.GetFromJsonAsync<List<Component>>($"project/{projectKey}/components", cancellationToken);

            var components = (response ?? [])
                .DistinctBy(component => component.Id)
                .ToImmutableHashSet();

            await cache.SetAsync(cacheKey, JsonSerializer.SerializeToUtf8Bytes(components), _cacheOptions.Components, cancellationToken);

            return components;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error fetching components for project {Project}", projectKey);
            throw;
        }
    }

    public async Task<ImmutableHashSet<string>> GetLabelsAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        const string cacheKey = "jira:labels:global";
        var cachedBytes = await cache.GetAsync(cacheKey, cancellationToken);

        if (cachedBytes is not null) 
            return JsonSerializer.Deserialize<ImmutableHashSet<string>>(cachedBytes) ?? [];

        try
        {
            var accumulator = new HashSet<string>();
            var startAt = 0;
            bool isLast;

            do
            {
                var response = await httpClient.GetFromJsonAsync<Label>($"label?startAt={startAt}", cancellationToken);
                
                if (response?.Values is not null)
                    foreach (var label in response.Values) 
                        accumulator.Add(label);

                isLast = response?.IsLast ?? true;
                
                if (response is not null) startAt += response.MaxResults;

            } while (!isLast);
            
            var labels = accumulator.ToImmutableHashSet();
            
            await cache.SetAsync(cacheKey, JsonSerializer.SerializeToUtf8Bytes(labels), _cacheOptions.Labels, cancellationToken);

            return labels;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error fetching global labels");
            throw;
        }
    }
}