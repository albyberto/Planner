namespace Planner.Models;

public class JiraFilterSettings
{
    public const string SectionName = "JiraFilters";

    public List<string> DefaultStatuses { get; init; } = [];
    public List<TeamMember> TeamMembers { get; init; } = [];
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

