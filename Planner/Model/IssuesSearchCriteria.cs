using System.Collections.Immutable;
using Planner.Components.Shared.Filters.Model;

namespace Planner.Model;

public record IssuesSearchCriteria
{
    public static IssuesSearchCriteria Empty => new();

    public static IssuesSearchCriteria Create(ProjectModel project) => new() { Project = project };
    
    public ProjectModel? Project { get; init; }

    public IFilterSelection<TypeModel> Types { get; init; } = FilterSelection<TypeModel>.Empty;
    public IFilterSelection<StatusModel> Statuses { get; init; } = FilterSelection<StatusModel>.Empty;
    public IFilterSelection<UserModel> Assignees { get; init; } = FilterSelection<UserModel>.Empty;
    public bool IncludeUnassigned { get; init; } = false;
    public IFilterSelection<ComponentModel> Components { get; init; } = FilterSelection<ComponentModel>.Empty;
    public IFilterSelection<LabelModel> Labels { get; init; } = FilterSelection<LabelModel>.Empty;

    public ImmutableDictionary<string, DatePreset.Preset> DateFilters { get; private init; } = ImmutableDictionary<string, DatePreset.Preset>.Empty;
    
    // NUOVO: Proprietà tipizzate per le transizioni, abbandonando il Dictionary
    public TransitionFilter<StatusModel> StatusTransition { get; init; } = new();
    public TransitionFilter<UserModel> AssigneeTransition { get; init; } = new();
    
    public IssuesSearchCriteria SetDateFilter(string field, DatePreset.Preset preset) =>
        preset == DatePreset.Preset.None 
            ? this with { DateFilters = DateFilters.Remove(field) } 
            : this with { DateFilters = DateFilters.SetItem(field, preset) };

    // NUOVO: Metodi helper specifici per le transizioni
    public IssuesSearchCriteria SetStatusTransition(TransitionFilter<StatusModel> filter) =>
        this with { StatusTransition = filter };

    public IssuesSearchCriteria SetAssigneeTransition(TransitionFilter<UserModel> filter) =>
        this with { AssigneeTransition = filter };
}