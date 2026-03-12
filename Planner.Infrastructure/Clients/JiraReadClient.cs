using System.Collections.Immutable;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Planner.Domain;

namespace Planner.Infrastructure.Clients;

/// <summary>
///     Client responsible for reading issues and their available transitions from Jira.
/// </summary>
public class JiraReadClient(HttpClient client, ILogger<JiraReadClient> logger)
{
    // A default fallback list of fields to fetch if none are provided
    private static readonly string[] DefaultFields =
    [
        "summary", "status", "assignee", "fixVersions",
        "created", "updated", "issuetype", "components",
        "labels", "timetracking", "worklog", "customfield_10117", "project", "comment"
    ];

    /// <summary>
    ///     Fetches issues using JQL, optionally specifying fields and expand parameters.
    /// </summary>
    /// <param name="jql">The Jira Query Language string.</param>
    /// <param name="fields">Optional list of fields to retrieve. If null, standard defaults are used.</param>
    /// <param name="expand">Optional list of properties to expand (e.g., "transitions", "changelog").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<ImmutableHashSet<Issue>> GetIssuesAsync(string jql, IEnumerable<string>? fields = null, IEnumerable<string>? expand = null, CancellationToken cancellationToken = default)
    {
        var accumulator = new List<Issue>();
        string? nextPageToken = null;

        var requestedFields = fields?.ToArray() ?? DefaultFields;
        var requestedExpand = expand?.ToArray() ?? [];

        try
        {
            do
            {
                var request = new Dictionary<string, object>
                {
                    ["jql"] = jql,
                    ["maxResults"] = 50,
                    ["fields"] = requestedFields
                };

                if (requestedExpand is { Length: > 0 }) 
                    request["expand"] = requestedExpand;

                if (!string.IsNullOrEmpty(nextPageToken)) 
                    request["nextPageToken"] = nextPageToken;

                var response = await client.PostAsJsonAsync("search/jql", request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    logger.LogError("Error from Jira API: {StatusCode} - {Error}", response.StatusCode, errorContent);
                    response.EnsureSuccessStatusCode();
                }

                var result = await response.Content.ReadFromJsonAsync<Issues>(cancellationToken);

                if (result is null || result.List.Count == 0) break;

                accumulator.AddRange(result.List);
                
                nextPageToken = result.NextPageToken;
                
            } while (!string.IsNullOrEmpty(nextPageToken));

            return accumulator.ToImmutableHashSet();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error fetching issues for JQL: {Jql}", jql);
            throw;
        }
    }
    
    public async Task<IReadOnlyList<Transition>> GetAvailableTransitionsAsync(string issueKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await client.GetFromJsonAsync<TransitionsResponse>($"issue/{issueKey}/transitions", cancellationToken);

            return response?.Transitions ?? [];
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error fetching transitions for issue {IssueKey}", issueKey);
            return [];
        }
    }
}