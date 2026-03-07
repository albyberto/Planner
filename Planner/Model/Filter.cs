namespace Planner.Model;

public record Filter(string? ProjectKey = null, HashSet<string>? Assignees = null, HashSet<string>? Statuses = null, HashSet<string>? Components = null, HashSet<string>? Labels = null, HashSet<string>? IssueTypes = null, bool IncludeUnassigned = false)
{
    public string? ProjectKey { get; set; } = ProjectKey;

    public HashSet<string> Assignees { get; set; } = Assignees ?? [];
    public HashSet<string> Statuses { get; set; } = Statuses ?? [];
    public HashSet<string> Components { get; set; } = Components ?? [];
    public HashSet<string> Labels { get; set; } = Labels ?? [];
    public HashSet<string> IssueTypes { get; set; } = IssueTypes ?? [];
    
    public bool IncludeUnassigned { get; set; } = IncludeUnassigned;
}