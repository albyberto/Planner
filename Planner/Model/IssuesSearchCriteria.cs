using System.Collections.Immutable;
using Planner.Components.Shared.Filters.Model;

namespace Planner.Model;

public record IssuesSearchCriteria
{
    public static IssuesSearchCriteria Empty => new();

    public static IssuesSearchCriteria Create(string key) => new() { Project = new(key) };
    
    public ProjectModel? Project { get; init; }

    public IFilterSelection<TypeModel> Types { get; init; } = FilterSelection<TypeModel>.Empty;
    public IFilterSelection<StatusModel> Statuses { get; init; } = FilterSelection<StatusModel>.Empty;
    public IFilterSelection<ComponentModel> Components { get; init; } = FilterSelection<ComponentModel>.Empty;
    public IFilterSelection<LabelModel> Labels { get; init; } = FilterSelection<LabelModel>.Empty;

    public ImmutableDictionary<string, DatePreset.Preset> DateFilters { get; private init; } = ImmutableDictionary<string, DatePreset.Preset>.Empty;
    
    public ImmutableDictionary<string, TransitionFilter<string>> TransitionFilters { get; private init; } = ImmutableDictionary<string, TransitionFilter<string>>.Empty;
    
    public IssuesSearchCriteria SetDateFilter(string field, DatePreset.Preset preset) =>
        preset == DatePreset.Preset.None 
            ? this with { DateFilters = DateFilters.Remove(field) } 
            : this with { DateFilters = DateFilters.SetItem(field, preset) };

    public IssuesSearchCriteria SetTransitionFilter(string field, TransitionFilter<string> filter) =>
        filter.Item is null && filter.Preset == DatePreset.Preset.None 
            ? this with { TransitionFilters = TransitionFilters.Remove(field) } 
            : this with { TransitionFilters = TransitionFilters.SetItem(field, filter) };
}