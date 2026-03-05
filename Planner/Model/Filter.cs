namespace Planner.Model;

public record Filter(string? ProjectKey = null, HashSet<string>? Assignees = null, HashSet<string>? Statuses = null, HashSet<string>? Components = null, HashSet<string>? Labels = null, HashSet<string>? IssueTypes = null, bool IncludeUnassigned = false)
{
    public string? ProjectKey { get; init; } = ProjectKey;

    public HashSet<string> Assignees { get; init; } = Assignees ?? [];
    public HashSet<string> Statuses { get; init; } = Statuses ?? [];
    public HashSet<string> Components { get; init; } = Components ?? [];
    public HashSet<string> Labels { get; init; } = Labels ?? [];
    public HashSet<string> IssueTypes { get; init; } = IssueTypes ?? [];
    
    public bool IncludeUnassigned { get; init; } = IncludeUnassigned;
}