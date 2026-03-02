using System.Text.Json.Serialization;

namespace Planner.Models;

/// <summary>
/// Risposta dalla Jira Search API POST /rest/api/3/search/jql
/// </summary>
public class JiraSearchResponse
{
    [JsonPropertyName("issues")]
    public List<JiraIssue> Issues { get; set; } = [];

    [JsonPropertyName("nextPageToken")]
    public string? NextPageToken { get; set; }

    [JsonPropertyName("isLast")]
    public bool IsLast { get; set; }
}

public class JiraIssue
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("self")]
    public string Self { get; set; } = string.Empty;

    [JsonPropertyName("fields")]
    public JiraIssueFields Fields { get; set; } = new();
}

public class JiraIssueFields
{
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public JiraStatus? Status { get; set; }

    [JsonPropertyName("assignee")]
    public JiraAssignee? Assignee { get; set; }

    [JsonPropertyName("timetracking")]
    public JiraTimeTracking? TimeTracking { get; set; }

    [JsonPropertyName("fixVersions")]
    public List<JiraFixVersion> FixVersions { get; set; } = [];

    [JsonPropertyName("components")]
    public List<JiraComponent> Components { get; set; } = [];

    [JsonPropertyName("labels")]
    public List<string> Labels { get; set; } = [];

    // Campi flat di comodo
    public string AccountId => Assignee?.AccountId ?? string.Empty;
    public string AssigneeName => Assignee?.DisplayName ?? "Non assegnato";
    public string AssigneeEmail => Assignee?.EmailAddress ?? string.Empty;
    public string StatusName => Status?.Name ?? string.Empty;
    public string FixVersionName => FixVersions.Count > 0 ? FixVersions[0].Name : "Nessuna Fix Version";
    public int? OriginalEstimateSeconds => TimeTracking?.OriginalEstimateSeconds;
    public int? RemainingEstimateSeconds => TimeTracking?.RemainingEstimateSeconds;
    public int? TimeSpentSeconds => TimeTracking?.TimeSpentSeconds;
}

public class JiraStatus
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class JiraAssignee
{
    [JsonPropertyName("accountId")]
    public string AccountId { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("emailAddress")]
    public string EmailAddress { get; set; } = string.Empty;
}

public class JiraFixVersion
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class JiraComponent
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class JiraTimeTracking
{
    [JsonPropertyName("originalEstimateSeconds")]
    public int? OriginalEstimateSeconds { get; set; }

    [JsonPropertyName("remainingEstimateSeconds")]
    public int? RemainingEstimateSeconds { get; set; }

    [JsonPropertyName("timeSpentSeconds")]
    public int? TimeSpentSeconds { get; set; }
}
