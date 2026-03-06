using System.Text.Json.Serialization;

namespace Planner.Domain;

public record Fields(
    [property: JsonPropertyName("summary")] string Summary,
    [property: JsonPropertyName("issuetype")] IssueType IssueType,
    [property: JsonPropertyName("components")] IReadOnlyList<Component> Components,
    [property: JsonPropertyName("created"), JsonConverter(typeof(JiraDateOnlyConverter))] DateOnly? Created,
    [property: JsonPropertyName("worklog")] Worklog Worklog,
    [property: JsonPropertyName("assignee")] User Assignee,
    [property: JsonPropertyName("fixVersions")] IReadOnlyList<FixVersion> FixVersions,
    [property: JsonPropertyName("updated"), JsonConverter(typeof(JiraDateOnlyConverter))] DateOnly? Updated,
    [property: JsonPropertyName("status")] Status Status,
    [property: JsonPropertyName("timetracking")] TimeTracking TimeTracking,
    [property: JsonPropertyName("labels")] IReadOnlyList<string> Labels,
    [property: JsonPropertyName("customfield_10117")] DateOnly? StartDate,
    [property: JsonPropertyName("project")] Project Project
);