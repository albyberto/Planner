using System.Collections.Immutable;
using Planner.Components.Shared.Filters.Model;
using Planner.Infrastructure.Domain.Enums;

namespace Planner.Model;

public record SearchCriteria
{
    public static SearchCriteria Empty => new();

    public ProjectModel? Project { get; init; }

    public IFilterSelection<TypeModel> Types { get; init; } = FilterSelection<TypeModel>.Empty;
    public IFilterSelection<StatusModel> Statuses { get; init; } = FilterSelection<StatusModel>.Empty;
    public IFilterSelection<UserModel> Assignees { get; init; } = FilterSelection<UserModel>.Empty;
    public bool IncludeUnassigned { get; init; }
    public IFilterSelection<ComponentModel> Components { get; init; } = FilterSelection<ComponentModel>.Empty;
    public IFilterSelection<LabelModel> Labels { get; init; } = FilterSelection<LabelModel>.Empty;

    public ImmutableDictionary<string, Preset> DateFilters { get; init; } = ImmutableDictionary<string, Preset>.Empty;

    public TransitionFilter<StatusModel> StatusTransition { get; init; } = new();

    public static SearchCriteria Create(ProjectModel project) => new() { Project = project };

    public SearchCriteria SetDateFilter(string field, Preset preset) =>
        preset == Preset.None ? this with { DateFilters = DateFilters.Remove(field) } : this with { DateFilters = DateFilters.SetItem(field, preset) };

    public SearchCriteria SetStatusTransition(TransitionFilter<StatusModel> filter) =>
        this with { StatusTransition = filter };
}