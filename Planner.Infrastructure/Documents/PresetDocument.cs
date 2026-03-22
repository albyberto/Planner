using Planner.Infrastructure.Domain;

namespace Planner.Infrastructure.Documents;

public record PresetDocument(Guid Id, string Name, string Description, bool Default, CriteriaDocument Criteria, DateTime Created, DateTime? Updated = null);

public record CriteriaDocument
{
    public string? ProjectKey { get; init; }
    
    public FilterSelectionDocument Types { get; init; } = new();
    public FilterSelectionDocument Statuses { get; init; } = new();
    public FilterSelectionDocument Assignees { get; init; } = new();
    public FilterSelectionDocument Components { get; init; } = new();
    public FilterSelectionDocument Labels { get; init; } = new();

    public bool IncludeUnassigned { get; init; }
    
    public Dictionary<string, string> DateFilters { get; init; } = new();
    
    public TransitionDocument? StatusTransition { get; init; }
}

public record TransitionDocument(string Field, string Preset);