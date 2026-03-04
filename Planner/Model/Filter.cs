namespace Planner.Model;

public record Filter(HashSet<string>? Assignees = null, HashSet<string>? Statuses = null, bool IncludeUnassigned = false)
{
    public HashSet<string> Assignees { get; init; } = Assignees != null ? Assignees : [];
    public HashSet<string> Statuses { get; init; } = Statuses != null ? Statuses : [];
    public bool IncludeUnassigned { get; init; } = IncludeUnassigned;
}
