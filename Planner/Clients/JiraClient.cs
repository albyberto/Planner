using System.Collections.Frozen;
using System.Collections.Immutable;
using Microsoft.Extensions.Options;
using Planner.Domain;
using Planner.Models;
using Type = Planner.Domain.Type;

namespace Planner.Clients;

public class JiraClient(HttpClient httpClient, IOptions<JiraQueryOptions> options, ILogger<JiraClient> logger)
{
    private readonly JiraQueryOptions _settings = options.Value;

    public async Task<ImmutableList<Status>> GetProjectStatusesAsync(CancellationToken cancellationToken = default)
    {
        var tasks = _settings.ProjectKeys.Select(async project =>
        {
            try
            {
                var issueTypes = await httpClient.GetFromJsonAsync<List<Type>>($"project/{project}/statuses", cancellationToken);
                return issueTypes ?? [];   
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error in GetProjectStatusesAsync");
                throw;
            }
        });

        var results = await Task.WhenAll(tasks);

        return results
            .SelectMany(requests => requests)
            .SelectMany(request => request.Statuses)
            .DistinctBy(status => status.Id)
            .OrderBy(status => status.StatusCategory.Id)
            .ThenBy(status => status.Id)
            .ToImmutableList();
    }

    public async Task<ImmutableList<User>> GetProjectAssigneesAsync(CancellationToken cancellationToken = default)
    {
        var tasks = _settings.ProjectKeys.Select(async projectKey =>
        {
            try
            {
                var assignees = await httpClient.GetFromJsonAsync<List<User>>($"user/assignable/search?project={projectKey}", cancellationToken);
                return assignees ?? [];
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error in GetProjectAssigneesAsync");
                throw;
            }
        });

        var results = await Task.WhenAll(tasks);

        return results
            .SelectMany(users => users)
            .Where(user => !string.IsNullOrEmpty(user.EmailAddress))
            .DistinctBy(user => user.AccountId)
            .OrderBy(user => user.DisplayName)
            .ToImmutableList();
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
                        "labels", "timetracking", "worklog" 
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

                if (searchResult?.List == null || searchResult.List.Count == 0)
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
}