namespace Planner.Model;

public record Filter(HashSet<string>? Assignees = null, HashSet<string>? Statuses = null)
{
    public HashSet<string> Assignees { get; init; } = Assignees ?? [];
    public HashSet<string> Statuses { get; init; } = Statuses ?? [];
}
