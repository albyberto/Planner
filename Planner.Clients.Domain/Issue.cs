using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Planner.Clients.Domain;

public class Issue
{
    [JsonPropertyName("id")] public required string Id { get; init; }
    [JsonPropertyName("key")] public required string Key { get; init; }
    [JsonPropertyName("self")] public string? Self { get; init; }
    [JsonPropertyName("expand")] public string? Expand { get; init; }
    [JsonPropertyName("transitions")] public ImmutableArray<Transition> Transitions { get; init; } = [];
    [JsonPropertyName("fields")] public Fields? Fields { get; init; }
}

public class Fields
{
    [JsonPropertyName("summary")] public string? Summary { get; init; }
    [JsonPropertyName("created")] public DateOnly? Created { get; init; }
    [JsonPropertyName("updated")] public DateOnly? Updated { get; init; }
    [JsonPropertyName("duedate")] public DateOnly? DueDate { get; init; }
    [JsonPropertyName("customfield_10117")] public DateOnly? StartDate { get; init; }
    [JsonPropertyName("customfield_10118")] public DateOnly? EndDate { get; init; }
    [JsonPropertyName("components")] public ImmutableArray<Component> Components { get; init; } = [];
    [JsonPropertyName("labels")] public ImmutableArray<string> Labels { get; init; } = [];
    [JsonPropertyName("fixVersions")] public ImmutableArray<FixVersion> FixVersions { get; init; } = [];
    [JsonPropertyName("issuetype")] public Type? Type { get; init; }
    [JsonPropertyName("status")] public Status? Status { get; init; }
    [JsonPropertyName("project")] public Project? Project { get; init; }
    [JsonPropertyName("assignee")] public User? Assignee { get; init; }
    [JsonPropertyName("timetracking")] public TimeTracking? TimeTracking { get; init; }
    [JsonPropertyName("worklog")] public Worklog? Worklog { get; init; }
    [JsonPropertyName("comment")] public Comment? Comment { get; init; }
}