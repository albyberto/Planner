namespace Planner.Model;

using System.Collections.Immutable;

public record IssueSearchCriteria(
    string? ProjectKey = null,
    ImmutableHashSet<string>? Assignees = null,
    ImmutableHashSet<string>? Statuses = null,
    ImmutableHashSet<string>? Components = null,
    ImmutableHashSet<string>? Labels = null,
    ImmutableHashSet<string>? Types = null,
    bool IncludeUnassigned = false)
{
    public ImmutableHashSet<string> Assignees { get; init; } = Assignees ?? [];
    public ImmutableHashSet<string> Statuses { get; init; } = Statuses ?? [];
    public ImmutableHashSet<string> Components { get; init; } = Components ?? [];
    public ImmutableHashSet<string> Labels { get; init; } = Labels ?? [];
    public ImmutableHashSet<string> Types { get; init; } = Types ?? [];

    public static IssueSearchCriteria Create(string projectKey) => new(ProjectKey: projectKey);
    public static IssueSearchCriteria Empty => new();
}