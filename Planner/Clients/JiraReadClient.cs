using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Planner.Domain;
using Planner.Models;
using Type = Planner.Domain.Type;

namespace Planner.Clients;

public class JiraReadClient(HttpClient httpClient, IOptions<JiraQueryOptions> options, IDistributedCache cache, ILogger<JiraReadClient> logger)
{
    private readonly JiraQueryOptions _settings = options.Value;

    public async Task<ImmutableList<Status>> GetProjectStatusesAsync(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "jira:statuses";

        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
            return JsonSerializer.Deserialize<ImmutableList<Status>>(cached) ?? [];

        var tasks = _settings.ProjectKeys.Select(async project =>
        {
            try
            {
                var issueTypes = await httpClient.GetFromJsonAsync<List<Type>>($"project/{project}/statuses", cancellationToken);
                return issueTypes != null ? issueTypes : [];   
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error in GetProjectStatusesAsync");
                throw;
            }
        });

        var results = await Task.WhenAll(tasks);

        var statuses = results
            .SelectMany(requests => requests)
            .SelectMany(request => request.Statuses)
            .DistinctBy(status => status.Id)
            .OrderBy(status => status.StatusCategory.Id)
            .ThenBy(status => status.Id)
            .ToImmutableList();

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1440)
        };
        
        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(statuses), cacheOptions, cancellationToken);
        return statuses;
    }

    public async Task<ImmutableList<User>> GetProjectAssigneesAsync(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "jira:assignees";

        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
            return JsonSerializer.Deserialize<ImmutableList<User>>(cached) ?? [];

        var tasks = _settings.ProjectKeys.Select(async projectKey =>
        {
            try
            {
                var assignees = await httpClient.GetFromJsonAsync<List<User>>($"user/assignable/search?project={projectKey}", cancellationToken);
                return assignees != null ? assignees : [];
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error in GetProjectAssigneesAsync");
                throw;
            }
        });

        var results = await Task.WhenAll(tasks);

        var assignees = results
            .SelectMany(users => users)
            .Where(user => !string.IsNullOrEmpty(user.EmailAddress))
            .DistinctBy(user => user.AccountId)
            .OrderBy(user => user.DisplayName)
            .ToImmutableList();

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(16)
        };
        
        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(assignees), cacheOptions, cancellationToken);
        return assignees;
    }
    
    public async Task<ImmutableList<Issue>> GetIssuesAsync(string jql, CancellationToken cancellationToken = default)
    {
        var issues = new List<Issue>();
        string? nextPageToken = null;
        
        try
        {
            do
            {
                var requestBody = new Dictionary<string, object>
                {
                    ["jql"] = jql,
                    ["maxResults"] = 50,
                    ["fields"] = new[] 
                    { 
                        "summary", "status", "assignee", "fixVersions", 
                        "created", "updated", "issuetype", "components", 
                        "labels", "timetracking", "worklog", "customfield_10117"
                    }
                };

                if (!string.IsNullOrEmpty(nextPageToken))
                {
                    requestBody["nextPageToken"] = nextPageToken;
                }

                var response = await httpClient.PostAsJsonAsync("search/jql", requestBody, cancellationToken);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    logger.LogError("Errore dalla API Jira: {StatusCode} - {Error}", response.StatusCode, errorContent);
                    response.EnsureSuccessStatusCode();
                }

                var searchResult = await response.Content.ReadFromJsonAsync<Issues>(cancellationToken: cancellationToken);

                if ((searchResult != null ? searchResult.List == null : true) || searchResult.List.Count == 0)
                {
                    break;
                }

                issues.AddRange(searchResult.List);
                nextPageToken = searchResult.NextPageToken;

            } while (!string.IsNullOrEmpty(nextPageToken));

            return issues.ToImmutableList();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error in GetIssuesAsync");
            throw;
        }
    }

    public async Task<IReadOnlyList<Transition>> GetTransitionsAsync(string issueKey, CancellationToken ct = default)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<TransitionsResponse>($"issue/{issueKey}/transitions", ct);
            return result?.Transitions ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetTransitionsAsync({IssueKey})", issueKey);
            throw;
        }
    }

    public async Task<ImmutableList<Component>> GetProjectComponentsAsync(CancellationToken ct = default)
    {
        const string cacheKey = "jira:components";

        var cached = await cache.GetStringAsync(cacheKey, ct);
        if (cached is not null)
            return JsonSerializer.Deserialize<ImmutableList<Component>>(cached) ?? [];

        var tasks = _settings.ProjectKeys.Select(async project =>
        {
            try
            {
                return await httpClient.GetFromJsonAsync<List<Component>>($"project/{project}/components", ct) ?? [];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching components for {Project}", project);
                throw;
            }
        });

        var results = await Task.WhenAll(tasks);

        var components = results
            .SelectMany(c => c)
            .DistinctBy(c => c.Id)
            .OrderBy(c => c.Name)
            .ToImmutableList();

        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(components),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8) }, ct);

        return components;
    }

    public async Task<ImmutableList<string>> GetProjectLabelsAsync(CancellationToken ct = default)
    {
        const string cacheKey = "jira:labels";

        var cached = await cache.GetStringAsync(cacheKey, ct);
        if (cached is not null)
            return JsonSerializer.Deserialize<ImmutableList<string>>(cached) ?? [];

        var allLabels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var project in _settings.ProjectKeys)
        {
            try
            {
                var body = new Dictionary<string, object>
                {
                    ["jql"] = $"project = {project} AND labels IS NOT EMPTY ORDER BY updated DESC",
                    ["maxResults"] = 100,
                    ["fields"] = new[] { "labels" }
                };

                var response = await httpClient.PostAsJsonAsync("search/jql", body, ct);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<Issues>(cancellationToken: ct);
                    if (result?.List is not null)
                    {
                        foreach (var label in result.List.SelectMany(i => i.Fields.Labels))
                            allLabels.Add(label);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching labels for {Project}", project);
            }
        }

        var labels = allLabels.OrderBy(l => l).ToImmutableList();

        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(labels),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8) }, ct);

        return labels;
    }

    public async Task<ImmutableList<IssueType>> GetProjectIssueTypesAsync(CancellationToken ct = default)
    {
        const string cacheKey = "jira:issuetypes";

        var cached = await cache.GetStringAsync(cacheKey, ct);
        if (cached is not null)
            return JsonSerializer.Deserialize<ImmutableList<IssueType>>(cached) ?? [];

        var tasks = _settings.ProjectKeys.Select(async project =>
        {
            try
            {
                var projectDetail = await httpClient.GetFromJsonAsync<JsonElement>($"project/{project}", ct);
                if (projectDetail.TryGetProperty("issueTypes", out var issueTypesElement))
                {
                    return JsonSerializer.Deserialize<List<IssueType>>(issueTypesElement.GetRawText()) ?? [];
                }
                return new List<IssueType>();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching issue types for {Project}", project);
                throw;
            }
        });

        var results = await Task.WhenAll(tasks);

        var issueTypes = results
            .SelectMany(t => t)
            .Where(t => t.Subtask != true)
            .DistinctBy(t => t.Id)
            .OrderBy(t => t.Name)
            .ToImmutableList();

        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(issueTypes),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8) }, ct);

        return issueTypes;
    }
}