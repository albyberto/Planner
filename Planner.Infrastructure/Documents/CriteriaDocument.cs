namespace Planner.Infrastructure.Documents;



public record CriteriaDocument
{
    public string? ProjectKey { get; init; }
    
    public (IEnumerable<string> Included, IEnumerable<string> Excluded) Types { get; init; } = new();
    public (IEnumerable<string> Included, IEnumerable<string> Excluded) Statuses { get; init; } = new();
    public (IEnumerable<string> Included, IEnumerable<string> Excluded) Assignees { get; init; } = new();
    public (IEnumerable<string> Included, IEnumerable<string> Excluded) Components { get; init; } = new();
    public (IEnumerable<string> Included, IEnumerable<string> Excluded) Labels { get; init; } = new();

    public bool IncludeUnassigned { get; init; }
    
    public Dictionary<string, string> DateFilters { get; init; } = new();
    
    public (string Field, string Preset) StatusTransition { get; init; }
    public (string Field, string Preset) AssigneeTransition { get; init; }
}