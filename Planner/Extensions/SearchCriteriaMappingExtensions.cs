using System.Collections.Immutable;
using Planner.Components.Shared.Filters.Model;
using Planner.Infrastructure.Domain;
using Planner.Infrastructure.Domain.Enums;
using Planner.Model;

namespace Planner.Extensions;

public static class SearchCriteriaMappingExtensions
{
    public static PresetItem ToPresetItem(this SearchCriteria criteria, Guid id, string name, string description) =>
        new()
        {
            Id = id,
            Name = name,
            Description = description,
            ProjectKey = criteria.Project?.Key,

            Types = criteria.Types.ToDocument(t => t.Name),
            Statuses = criteria.Statuses.ToDocument(s => s.Name),
            Assignees = criteria.Assignees.ToDocument(a => a.EmailAddress),
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
        };

    private static FilterSelectionDocument ToDocument<T>(this IFilterSelection<T> selection, Func<T, string> keySelector) =>
        new()
        {
            Included = selection.Included.Select(keySelector).ToImmutableHashSet(),
            Excluded = selection.Excluded.Select(keySelector).ToImmutableHashSet()
        };
}