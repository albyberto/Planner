using System.Collections.Immutable;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Planner.Clients.Domain;
using Planner.Clients.Domain.Responses;

namespace Planner.Clients;

public class JiraReadClient(HttpClient client, IOptionsMonitor<JsonSerializerOptions> jsonOptions, ILogger<JiraReadClient> logger)
{
    private readonly JsonSerializerOptions _jsonOptions = jsonOptions.Get("JiraSerializer");
    
    public async Task<ImmutableHashSet<Issue>> GetIssuesAsync(string jql, string[]? fields = null, string[]? expand = null, CancellationToken cancellationToken = default)
    {
        var accumulator = new List<Issue>();
        string? nextPageToken = null;

        try
        {
            do
            {
                Dictionary<string, object> request = new()
                    {
                        ["jql"] = jql,
                        ["maxResults"] = 50
                    };

                if(fields is { Length: > 0 })
                    request["fields"] = fields;
                    
                if (expand is { Length: > 0 })
                    request["expand"] = string.Join(',', expand);

                if (!string.IsNullOrEmpty(nextPageToken))
                    request["nextPageToken"] = nextPageToken;

                var response = await client.PostAsJsonAsync("search/jql", request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    logger.LogError("Error from Jira API: {StatusCode} - {Error}", response.StatusCode, errorContent);
                    response.EnsureSuccessStatusCode();
                }

                var result = await response.Content.ReadFromJsonAsync<IssuesResponse>(_jsonOptions, cancellationToken);

                if (result is null || result.Issues.Length == 0) break;

                accumulator.AddRange(result.Issues);
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
}