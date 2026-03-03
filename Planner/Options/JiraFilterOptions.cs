namespace Planner.Options;

public class JiraFilterOptions
{
    public const string SectionName = "JiraFilters";

    public HashSet<string> DefaultStatuses { get; init; } = [];
    public HashSet<TeamMember> TeamMembers { get; init; } = [];
    public bool IncludeUnassignedByDefault { get; init; }
    public List<JiraPreset> Presets { get; init; } = [];
}

public class TeamMember
{
    public string Email { get; init; } = string.Empty;
    public bool IsDefault { get; init; } = true;
}

public class JiraPreset
{
    public string Name { get; init; } = string.Empty;
    public string Jql { get; init; } = string.Empty;
}

