using Planner.Infrastructure.Domain.Enums;

namespace Planner.Infrastructure.Domain;

public record PresetItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool Default { get; init; }
    
    public string? ProjectKey { get; init; }
    public FilterSelectionDocument Types { get; init; } = new();
    public FilterSelectionDocument Statuses { get; init; } = new();
    public FilterSelectionDocument Assignees { get; init; } = new();
    public bool IncludeUnassigned { get; init; }
    public FilterSelectionDocument Components { get; init; } = new();
    public FilterSelectionDocument Labels { get; init; } = new();
    
    public Dictionary<string, Preset> DateFilters { get; init; } = new();
    
    public TransitionFilterDocument? StatusTransition { get; init; }
    public TransitionFilterDocument? AssigneeTransition { get; init; }
}