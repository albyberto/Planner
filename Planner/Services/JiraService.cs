using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Planner.Models;

namespace Planner.Services;

/// <summary>
/// Servizio per comunicare con le API REST di Jira Cloud.
/// </summary>
public class JiraService(
    HttpClient httpClient,
    IOptions<JiraQuerySettings> querySettings,
    IOptions<JiraFilterSettings> filterSettings,
    ILogger<JiraService> logger)
{
    private readonly JiraQuerySettings _query = querySettings.Value;
    private readonly JiraFilterSettings _filters = filterSettings.Value;
    private const string UnassignedKey = "__UNASSIGNED__";

    public string GetUnassignedKey() => UnassignedKey;

    public List<string> GetTeamMembers() => _filters.TeamMembers.Select(m => m.Email).ToList();

    public List<string> GetDefaultTeamEmails() => _filters.TeamMembers.Where(m => m.IsDefault).Select(m => m.Email).ToList();

    public List<string> GetDefaultStatuses() => _filters.DefaultStatuses;

    public bool IsUnassignedDefault() => _filters.IncludeUnassignedByDefault;

    public List<JiraPreset> GetPresets() => _filters.Presets;

    /// <summary>
    /// Recupera tutti gli stati disponibili per i progetti configurati.
    /// </summary>
    public async Task<List<string>> GetProjectStatusesAsync()
    {
        var allStatuses = new HashSet<string>();

        foreach (var projectKey in _query.ProjectKeys)
        {
            try
            {
                var response = await httpClient.GetAsync($"rest/api/3/project/{projectKey}/statuses");
                if (!response.IsSuccessStatusCode) continue;

                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);

                foreach (var issueType in doc.RootElement.EnumerateArray())
                {
                    if (issueType.TryGetProperty("statuses", out var statuses))
                    {
                        foreach (var status in statuses.EnumerateArray())
                        {
                            if (status.TryGetProperty("name", out var name))
                                allStatuses.Add(name.GetString() ?? string.Empty);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Errore recuperando gli stati per il progetto {Project}", projectKey);
            }
        }

        return allStatuses.Where(s => !string.IsNullOrEmpty(s)).OrderBy(s => s).ToList();
    }

    /// <summary>
    /// Recupera issue per stati e assignee.
    /// Se assigneeEmails contiene __UNASSIGNED__, include anche le issue senza assegnatario.
    /// </summary>
    public async Task<List<JiraIssue>> GetIssuesByStatusesAsync(
        IEnumerable<string> statuses,
        IEnumerable<string>? assigneeEmails = null,
        bool unassignedOnly = false,
        int maxResults = 50)
    {
        var statusList = statuses.ToList();
        if (statusList.Count == 0) return [];

        var formattedProjects = string.Join(", ", _query.ProjectKeys.Select(k => $"\"{k}\""));
        var formattedStatuses = string.Join(", ", statusList.Select(s => $"\"{s}\""));
        var jql = $"project IN ({formattedProjects}) AND status IN ({formattedStatuses})";

        if (unassignedOnly)
        {
            jql += " AND assignee is EMPTY";
        }
        else
        {
            var clause = BuildAssigneeClause(assigneeEmails);
            if (!string.IsNullOrEmpty(clause))
                jql += $" AND {clause}";
        }

        jql += " ORDER BY updated DESC";
        return await ExecuteJqlAsync(jql, maxResults);
    }

    /// <summary>
    /// Esegue un preset: project + assignee dal filtro corrente vengono aggiunti automaticamente.
    /// La JQL del preset è solo la condizione aggiuntiva (es. "sprint in openSprints()").
    /// </summary>
    public async Task<List<JiraIssue>> SearchByPresetAsync(
        string presetJql,
        IEnumerable<string>? assigneeEmails = null,
        int maxResults = 50)
    {
        var formattedProjects = string.Join(", ", _query.ProjectKeys.Select(k => $"\"{k}\""));
        var jql = $"project IN ({formattedProjects})";

        var clause = BuildAssigneeClause(assigneeEmails);
        if (!string.IsNullOrEmpty(clause))
            jql += $" AND {clause}";

        if (!string.IsNullOrEmpty(presetJql))
            jql += $" AND ({presetJql})";

        jql += " ORDER BY updated DESC";
        return await ExecuteJqlAsync(jql, maxResults);
    }

    /// <summary>
    /// Costruisce la clausola JQL per gli assignee.
    /// Se la lista contiene __UNASSIGNED__ e anche email => (assignee IN (...) OR assignee is EMPTY)
    /// Se solo __UNASSIGNED__ => assignee is EMPTY
    /// Se solo email => assignee IN (...)
    /// </summary>
    private string? BuildAssigneeClause(IEnumerable<string>? assigneeEmails)
    {
        if (assigneeEmails == null) return null;
        var list = assigneeEmails.ToList();
        if (list.Count == 0) return null;

        var includeUnassigned = list.Remove(UnassignedKey);
        var hasEmails = list.Count > 0;

        if (hasEmails && includeUnassigned)
        {
            var formatted = string.Join(", ", list.Select(a => $"\"{a}\""));
            return $"(assignee IN ({formatted}) OR assignee is EMPTY)";
        }
        if (hasEmails)
        {
            var formatted = string.Join(", ", list.Select(a => $"\"{a}\""));
            return $"assignee IN ({formatted})";
        }
        if (includeUnassigned)
            return "assignee is EMPTY";

        return null;
    }

    /// <summary>
    /// Esegue una query JQL con paginazione nextPageToken.
    /// </summary>
    private async Task<List<JiraIssue>> ExecuteJqlAsync(string jql, int maxResults = 50)
    {
        var allIssues = new List<JiraIssue>();
        string? nextPageToken = null;

        logger.LogInformation("Esecuzione query JQL: {Jql}", jql);

        do
        {
            var requestBody = new Dictionary<string, object>
            {
                ["jql"] = jql,
                ["fields"] = new[] { "summary", "status", "assignee", "timetracking", "fixVersions", "components", "labels" },
                ["fieldsByKeys"] = false,
                ["maxResults"] = maxResults
            };

            if (nextPageToken != null)
                requestBody["nextPageToken"] = nextPageToken;

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("rest/api/3/search/jql", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                logger.LogError("Errore dalla Jira API: {StatusCode} - {Error}", response.StatusCode, errorContent);
                throw new HttpRequestException($"Errore Jira API: {response.StatusCode} - {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var searchResponse = JsonSerializer.Deserialize<JiraSearchResponse>(content);

            if (searchResponse == null || searchResponse.Issues.Count == 0)
                break;

            allIssues.AddRange(searchResponse.Issues);
            nextPageToken = searchResponse.IsLast ? null : searchResponse.NextPageToken;

        } while (nextPageToken != null);

        logger.LogInformation("Trovate {Count} issue", allIssues.Count);
        return allIssues;
    }
}
