using System.Collections.Immutable;
using Planner.Components.Shared.Filters.Model;

namespace Planner.Model;

public record IssuesSearchCriteria
{
    public static IssuesSearchCriteria Empty => new();

    public ProjectModel? ProjectKey { get; init; }

    public IFilterSelection<TypeModel> Types { get; init; } = FilterSelection<TypeModel>.Empty;
    public IFilterSelection<StatusModel> Statuses { get; init; } = FilterSelection<StatusModel>.Empty;
    public IFilterSelection<ComponentModel> Components { get; init; } = FilterSelection<ComponentModel>.Empty;
    public IFilterSelection<LabelModel> Labels { get; init; } = FilterSelection<LabelModel>.Empty;

    public ImmutableDictionary<string, DatePreset> DateFilters { get; private init; } = [];
    public static IssuesSearchCriteria Create(string key) => new() { ProjectKey = new(key) };

    public IssuesSearchCriteria SetDateFilter(string field, DatePreset preset) =>
        preset == DatePreset.None 
            ? this with { DateFilters = DateFilters.Remove(field) } 
            : this with { DateFilters = DateFilters.SetItem(field, preset) };
}