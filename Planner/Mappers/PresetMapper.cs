using System.Collections.Immutable;
using Planner.Components.Shared.Filters.Model;
using Planner.Infrastructure.Domain;
using Planner.Infrastructure.Domain.Enums;
using Planner.Model;

namespace Planner.Mappers;

// ------------------------------------
// --- UI => Domain -------------------
// ------------------------------------
public static class PresetMapper
{
    public static PresetItem ToDomain(this SearchCriteria criteria, Guid id, string name, string description, bool isDefault = false) =>
        new()
        {
            Id = id,
            Name = name,
            Description = description,
            Default = isDefault,
            
            Criteria = new()
            {
                ProjectKey = criteria.Project?.Key,

                Types = criteria.Types.ToDocument(t => t.Name),
                Statuses = criteria.Statuses.ToDocument(s => s.Name),
                
                Assignees = criteria.Assignees.ToDocument(a => a.AccountId), 
                
                Components = criteria.Components.ToDocument(c => c.Name),
                Labels = criteria.Labels.ToDocument(l => l.Name),

                IncludeUnassigned = criteria.IncludeUnassigned,
                
                DateFilters = criteria.DateFilters.ToDictionary(k => k.Key, v => v.Value),

                StatusTransition = criteria.StatusTransition.Item is not null || criteria.StatusTransition.Preset != Preset.None
                    ? new TransitionFilterDocument
                    {
                        Field = criteria.StatusTransition.Item?.Name ?? string.Empty,
                        Preset = criteria.StatusTransition.Preset
                    }
                    : null
            }
        };

    private static FilterSelectionDocument ToDocument<T>(this IFilterSelection<T> selection, Func<T, string> keySelector) =>
        new()
        {
            Included = selection.Included.Select(keySelector).ToImmutableHashSet(),
            Excluded = selection.Excluded.Select(keySelector).ToImmutableHashSet()
        };

// ------------------------------------
// --- Domain => UI -------------------
// ------------------------------------
    public static SearchCriteria ToModel(
        this PresetItem preset, 
        ImmutableArray<ProjectModel> projects, 
        ImmutableArray<TypeModel> types, 
        ImmutableArray<UserModel> assignees, 
        ImmutableArray<ComponentModel> components, 
        ImmutableArray<LabelModel> labels)
    {
        var criteria = preset.Criteria;

        var project = projects.FirstOrDefault(p => 
            string.Equals(p.Key?.Trim(), criteria.ProjectKey?.Trim(), StringComparison.OrdinalIgnoreCase));

        return new()
        {
            Project = project,
            IncludeUnassigned = criteria.IncludeUnassigned,

            Types = MapSelection(types, criteria.Types, t => t.Name),
            Assignees = MapSelection(assignees, criteria.Assignees, a => a.AccountId), 
            Components = MapSelection(components, criteria.Components, c => c.Name),
            Labels = MapSelection(labels, criteria.Labels, l => l.Name),

            DateFilters = criteria.DateFilters.ToImmutableDictionary(k => k.Key, v => v.Value),

            Statuses = FilterSelection<StatusModel>.Empty,
            StatusTransition = new()
        };
    }

    public static SearchCriteria HydrateStatuses(this SearchCriteria searchCriteria, PresetItem preset, ImmutableArray<StatusModel> availableStatuses)
    {
        var criteria = preset.Criteria;

        var hydratedStatuses = MapSelection(availableStatuses, criteria.Statuses, s => s.Name);

        var transitionItem = availableStatuses.FirstOrDefault(s => 
            string.Equals(s.Name, criteria.StatusTransition?.Field, StringComparison.OrdinalIgnoreCase));
            
        var hydratedTransition = new TransitionFilter<StatusModel>
        {
            Item = transitionItem,
            Preset = criteria.StatusTransition?.Preset ?? Preset.None
        };

        return searchCriteria with 
        { 
            Statuses = hydratedStatuses,
            StatusTransition = hydratedTransition
        };
    }

    private static IFilterSelection<T> MapSelection<T>(ImmutableArray<T> availableItems, FilterSelectionDocument document, Func<T, string> selector)
    {
        var included = availableItems
            .Where(item => document.Included.Contains(selector(item)))
            .ToImmutableHashSet();

        var excluded = availableItems
            .Where(item => document.Excluded.Contains(selector(item)))
            .ToImmutableHashSet();

        return new FilterSelection<T>(included, excluded);
    }
}