using System.Collections.Immutable;
using Planner.Infrastructure.Documents;
using Planner.Infrastructure.Domain;
using Planner.Infrastructure.Domain.Enums;

namespace Planner.Infrastructure.Mappers;

public static class PresetMappingExtensions
{
    private static FilterSelectionDocument MapToSelection(this (IEnumerable<string> Included, IEnumerable<string> Excluded) tuple) =>
        new()
        {
            Included = tuple.Included.ToImmutableHashSet(),
            Excluded = tuple.Excluded.ToImmutableHashSet()
        };

    extension(PresetItem item)
    {
        public PresetDocument ToDocument() =>
            new(item.Id == Guid.Empty ? Guid.NewGuid() : item.Id, item.Name, item.Description, item.Default, item.ToCriteriaDocument(), DateTime.UtcNow);

        private CriteriaDocument ToCriteriaDocument() =>
            new()
            {
                ProjectKey = item.ProjectKey,

                Types = (item.Types.Included, item.Types.Excluded),
                Statuses = (item.Statuses.Included, item.Statuses.Excluded),
                Assignees = (item.Assignees.Included, item.Assignees.Excluded),
                Components = (item.Components.Included, item.Components.Excluded),
                Labels = (item.Labels.Included, item.Labels.Excluded),

                IncludeUnassigned = item.IncludeUnassigned,

                DateFilters = item.DateFilters.ToDictionary(filterEntry => filterEntry.Key, filterEntry => $"{filterEntry.Value}"),

                StatusTransition = item.StatusTransition is not null ? (item.StatusTransition.Field, $"{item.StatusTransition.Preset}") : (string.Empty, nameof(Preset.None))
            };
    }

    extension(PresetDocument existingDocument)
    {
        public PresetDocument Merge(PresetItem updatedItem) =>
            new(existingDocument.Id, updatedItem.Name, updatedItem.Description, updatedItem.Default, updatedItem.ToCriteriaDocument(), existingDocument.Created, DateTime.UtcNow);

        public PresetItem ToItem() =>
            new()
            {
                Id = existingDocument.Id,
                Name = existingDocument.Name,
                ProjectKey = existingDocument.Criteria.ProjectKey,

                Types = existingDocument.Criteria.Types.MapToSelection(),
                Statuses = existingDocument.Criteria.Statuses.MapToSelection(),
                Assignees = existingDocument.Criteria.Assignees.MapToSelection(),
                Components = existingDocument.Criteria.Components.MapToSelection(),
                Labels = existingDocument.Criteria.Labels.MapToSelection(),

                IncludeUnassigned = existingDocument.Criteria.IncludeUnassigned,

                DateFilters = existingDocument.Criteria.DateFilters.ToDictionary(dateFilterEntry => dateFilterEntry.Key, dateFilterEntry => Enum.TryParse<Preset>(dateFilterEntry.Value, true, out var parsedPreset) ? parsedPreset : Preset.None),

                StatusTransition = !string.IsNullOrWhiteSpace(existingDocument.Criteria.StatusTransition.Field)
                    ? new TransitionFilterDocument
                    {
                        Field = existingDocument.Criteria.StatusTransition.Field,
                        Preset = Enum.TryParse<Preset>(existingDocument.Criteria.StatusTransition.Preset, true, out var parsedStatusPreset) ? parsedStatusPreset : Preset.None
                    }
                    : null
            };
    }
}