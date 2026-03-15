using System.ComponentModel.DataAnnotations;

namespace Planner.Options;

public record DefaultFilterItem
{
    public string Value { get; init; } = string.Empty;
    public bool IsExcluded { get; init; } = false;
}

public class JiraFilterOptions
{
    public const string SectionName = nameof(JiraFilterOptions);

    [Required, MinLength(2)]
    public string DefaultProject { get; init; } = string.Empty;
    
    public List<DefaultFilterItem> DefaultTypes { get; init; } = [];
    public List<DefaultFilterItem> DefaultStatuses { get; init; } = [];
    public List<DefaultFilterItem> DefaultAssignees { get; init; } = [];
    public List<DefaultFilterItem> DefaultComponents { get; init; } = [];
    public List<DefaultFilterItem> DefaultLabels { get; init; } = [];
    
    public bool IncludeUnassignedByDefault { get; init; }
    
    public string Me { get; init; } = string.Empty;
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