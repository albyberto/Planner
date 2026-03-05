using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Planner.Domain;
using Planner.Model;
using Planner.Models;
using Type = Planner.Domain.Type;

namespace Planner.Clients;

public class JiraReadClient(HttpClient httpClient, IOptions<JiraQueryOptions> options, IDistributedCache cache, ILogger<JiraReadClient> logger)
{
    private readonly JiraQueryOptions _settings = options.Value;

    #region Projects

    public IEnumerable<ProjectModel> GetProjects() =>
        _settings.ProjectKeys.Select(key => new ProjectModel(
            key,
            () => GetProjectDetailsAsync(key),
            () => GetStatusesForProjectAsync(key),
            () => GetAssigneesForProjectAsync(key),
            () => GetComponentsForProjectAsync(key),
            () => GetLabelsForProjectAsync(key),
            () => GetIssueTypesForProjectAsync(key)
        ));

    private async Task<Project> GetProjectDetailsAsync(string projectKey)
    {
        var cacheKey = $"jira:project:{projectKey}";
        var cached = await cache.GetStringAsync(cacheKey);
        if (cached is not null)
            return JsonSerializer.Deserialize<Project>(cached)!;

        try
        {
            var project = await httpClient.GetFromJsonAsync<Project>($"project/{projectKey}");
            if (project is not null)
                await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(project),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) });
            return project!;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching project details for {Project}", projectKey);
            throw;
        }
    }

    private async Task<ImmutableList<Status>> GetStatusesForProjectAsync(string projectKey)
    {
        var cacheKey = $"jira:statuses:{projectKey}";
        var cached = await cache.GetStringAsync(cacheKey);
        if (cached is not null)
            return JsonSerializer.Deserialize<ImmutableList<Status>>(cached) ?? [];

        try
        {
            var types = await httpClient.GetFromJsonAsync<List<Type>>($"project/{projectKey}/statuses");
            var statuses = (types ?? [])
                .SelectMany(t => t.Statuses)
                .DistinctBy(status => status.Id)
                .OrderBy(status => status.StatusCategory.Id)
                .ThenBy(status => status.Id)
                .ToImmutableList();

            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(statuses),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) });

            return statuses;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error fetching statuses for project {Project}", projectKey);
            throw;
        }
    }

    private async Task<ImmutableList<User>> GetAssigneesForProjectAsync(string projectKey)
    {
        var cacheKey = $"jira:assignees:{projectKey}";
        var cached = await cache.GetStringAsync(cacheKey);
        if (cached is not null)
            return JsonSerializer.Deserialize<ImmutableList<User>>(cached) ?? [];

        try
        {
            var assignees = await httpClient.GetFromJsonAsync<List<User>>($"user/assignable/search?project={projectKey}");
            var result = (assignees ?? [])
                .Where(user => !string.IsNullOrEmpty(user.EmailAddress))
                .DistinctBy(user => user.AccountId)
                .OrderBy(user => user.DisplayName)
                .ToImmutableList();

            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(16) });

            return result;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error fetching assignees for project {Project}", projectKey);
            throw;
        }
    }

    private async Task<ImmutableList<Component>> GetComponentsForProjectAsync(string projectKey)
    {
        var cacheKey = $"jira:components:{projectKey}";
        var cached = await cache.GetStringAsync(cacheKey);
        if (cached is not null)
            return JsonSerializer.Deserialize<ImmutableList<Component>>(cached) ?? [];

        try
        {
            var components = await httpClient.GetFromJsonAsync<List<Component>>($"project/{projectKey}/components");
            var result = (components ?? [])
                .DistinctBy(c => c.Id)
                .OrderBy(c => c.Name)
                .ToImmutableList();

            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8) });

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching components for {Project}", projectKey);
            throw;
        }
    }

    private async Task<ImmutableList<string>> GetLabelsForProjectAsync(string projectKey)
    {
        var cacheKey = $"jira:labels:{projectKey}";
        var cached = await cache.GetStringAsync(cacheKey);
        if (cached is not null)
            return JsonSerializer.Deserialize<ImmutableList<string>>(cached) ?? [];

        try
        {
            var body = new Dictionary<string, object>
            {
                ["jql"] = $"project = {projectKey} AND labels IS NOT EMPTY ORDER BY updated DESC",
                ["maxResults"] = 100,
                ["fields"] = new[] { "labels" }
            };

            var response = await httpClient.PostAsJsonAsync("search/jql", body);
            var labels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Issues>();
                if (result?.List is not null)
                    foreach (var label in result.List.SelectMany(i => i.Fields.Labels))
                        labels.Add(label);
            }

            var finalLabels = labels.OrderBy(l => l).ToImmutableList();

            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(finalLabels),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8) });

            return finalLabels;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching labels for {Project}", projectKey);
            throw;
        }
    }

    private async Task<ImmutableList<IssueType>> GetIssueTypesForProjectAsync(string projectKey)
    {
        var cacheKey = $"jira:issuetypes:{projectKey}";
        var cached = await cache.GetStringAsync(cacheKey);
        if (cached is not null)
            return JsonSerializer.Deserialize<ImmutableList<IssueType>>(cached) ?? [];

        try
        {
            var projectDetail = await httpClient.GetFromJsonAsync<JsonElement>($"project/{projectKey}");
            var issueTypes = new List<IssueType>();

            if (projectDetail.TryGetProperty("issueTypes", out var issueTypesElement)) issueTypes = JsonSerializer.Deserialize<List<IssueType>>(issueTypesElement.GetRawText()) ?? [];

            var result = issueTypes
                .Where(t => t.Subtask != true)
                .DistinctBy(t => t.Id)
                .OrderBy(t => t.Name)
                .ToImmutableList();

            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8) });

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching issue types for {Project}", projectKey);
            throw;
        }
    }

    #endregion

    #region Issues

    public async Task<IReadOnlyList<Transition>> GetTransitionsAsync(string issueKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<TransitionsResponse>($"issue/{issueKey}/transitions", cancellationToken);
            return result?.Transitions ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetTransitionsAsync({IssueKey})", issueKey);
            throw;
        }
    }

    public async Task<ImmutableList<IssueModel>> GetIssuesAsync(string jql, CancellationToken cancellationToken = default)
    {
        var accumulator = new List<Issue>();
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
                    requestBody["nextPageToken"] = nextPageToken;

                var response = await httpClient.PostAsJsonAsync("search/jql", requestBody, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    logger.LogError("Errore dalla API Jira: {StatusCode} - {Error}", response.StatusCode, errorContent);
                    response.EnsureSuccessStatusCode();
                }

                var result = await response.Content.ReadFromJsonAsync<Issues>(cancellationToken);

                if (result is null || result.List.Count == 0) break;

                accumulator.AddRange(result.List);
                nextPageToken = result.NextPageToken;
            } while (!string.IsNullOrEmpty(nextPageToken));

            return accumulator.Select(dto => new IssueModel(
                dto,
                () => GetTransitionsAsync(dto.Key, cancellationToken)
            )).ToImmutableList();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error in GetIssuesAsync");
            throw;
        }
    }

    #endregion
}