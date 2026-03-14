namespace Planner.Model;

using System.Collections.Immutable;

public record IssuesSearchCriteria(
    string? ProjectKey = null,
    HashSet<string>? Assignees = null,
    HashSet<string>? Statuses = null,
    HashSet<string>? Components = null,
    HashSet<string>? Labels = null,
    HashSet<string>? Types = null,
    bool IncludeUnassigned = false)
{
    public HashSet<string> Assignees { get; init; } = Assignees ?? [];
    public HashSet<string> Statuses { get; init; } = Statuses ?? [];
    public HashSet<string> Components { get; init; } = Components ?? [];
    public HashSet<string> Labels { get; init; } = Labels ?? [];
    public HashSet<string> Types { get; init; } = Types ?? [];

    public static IssuesSearchCriteria Create(string projectKey) => new(ProjectKey: projectKey);
    public static IssuesSearchCriteria Empty => new();
}