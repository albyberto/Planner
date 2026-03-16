using Planner.Components.Shared.Filters.Model;

namespace Planner.Model;

public record IssuesSearchCriteria
{
    public string ProjectKey { get; init; } = string.Empty;
    
    public IFilterSelection<ComponentModel> Components { get; init; } = FilterSelection<ComponentModel>.Empty;
    

    public static IssuesSearchCriteria Empty => new();
    
    public static IssuesSearchCriteria Create(string projectKey) => new() { ProjectKey = projectKey };
}