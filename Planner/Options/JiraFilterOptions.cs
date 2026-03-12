using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace Planner.Options;

public class JiraFilterOptions
{
    public const string SectionName = nameof(JiraFilterOptions);

    [Required, MinLength(2)]
    public string DefaultProject { get; init; } = string.Empty;
    public HashSet<string> DefaultTypes { get; init; } = [];
    public HashSet<string> DefaultStatuses { get; init; } = [];
    public HashSet<string> DefaultAssignees { get; init; } = [];
    public HashSet<string> DefaultComponents { get; init; } = [];
    public HashSet<string> DefaultLabels { get; init; } = [];
    
    public bool IncludeUnassignedByDefault { get; init; }
    
    public string Me { get; init; } = "";
}

public class TeamMember
{
    public string Email { get; init; } = string.Empty;
}

public class JiraPreset
{
    public string Name { get; init; } = string.Empty;
    public string Jql { get; init; } = string.Empty;
}

public class MeOptions
{
    public string Email { get; init; } = string.Empty;
}

