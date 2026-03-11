namespace Planner.Model;

public record FilterModel(string? ProjectKey = null, HashSet<string>? Assignees = null, HashSet<string>? Statuses = null, HashSet<string>? Components = null, HashSet<string>? Labels = null, HashSet<string>? Types = null, bool IncludeUnassigned = false)
{
    public string? ProjectKey { get; set; } = ProjectKey;

    public HashSet<string> Types { get; set; } = Types ?? [];
    public HashSet<string> Statuses { get; set; } = Statuses ?? [];
    public HashSet<string> Assignees { get; set; } = Assignees ?? [];
    public HashSet<string> Components { get; set; } = Components ?? [];
    public HashSet<string> Labels { get; set; } = Labels ?? [];

    public bool IncludeUnassigned { get; set; } = IncludeUnassigned;

    public void Clear(string projectKey)
    {
        ProjectKey = projectKey;
        Types.Clear();
        Statuses.Clear();
        Assignees.Clear();
        Components.Clear();
        Labels.Clear();
        IncludeUnassigned = false;
    }
}