using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace Planner.Options;

public class JiraFilterOptions
{
    public const string SectionName = nameof(JiraFilterOptions);

    [Required, MinLength(2)]
    public string DefaultProject { get; init; } = string.Empty;
    public ImmutableHashSet<string> DefaultTypes { get; init; } = [];
    public ImmutableHashSet<string> DefaultStatuses { get; init; } = [];
    public ImmutableHashSet<string> DefaultAssignees { get; init; } = [];
    public ImmutableHashSet<string> DefaultComponents { get; init; } = [];
    public ImmutableHashSet<string> DefaultLabels { get; init; } = [];
    
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

