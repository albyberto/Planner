using Planner.Infrastructure.Documents;
using Planner.Infrastructure.Domain;
using Planner.Infrastructure.Domain.Enums;

namespace Planner.Infrastructure.Mappers;

public static class PresetMapper
{
    extension(PresetItem item)
    {
        public PresetDocument ToDocument() =>
            new(item.Id == Guid.Empty ? Guid.NewGuid() : item.Id, item.Name, item.Description, item.Default, item.ToCriteriaDocument(), item.Created == default ? DateTime.UtcNow : item.Created, DateTime.UtcNow);

        private CriteriaDocument ToCriteriaDocument() =>
            new()
            {
                ProjectKey = item.Criteria.ProjectKey,

                Types = item.Criteria.Types,
                Statuses = item.Criteria.Statuses,
                Assignees = item.Criteria.Assignees,
                Components = item.Criteria.Components,
                Labels = item.Criteria.Labels,

                IncludeUnassigned = item.Criteria.IncludeUnassigned,

                DateFilters = item.Criteria.DateFilters.ToDictionary(k => k.Key, v => v.Value.ToString()),

                StatusTransition = item.Criteria.StatusTransition is not null ? new TransitionDocument(item.Criteria.StatusTransition.Field, item.Criteria.StatusTransition.Preset.ToString()) : null
            };
    }

    extension(PresetDocument document)
    {
        public PresetDocument Merge(PresetItem updatedItem) =>
            new(document.Id, updatedItem.Name, updatedItem.Description, updatedItem.Default, updatedItem.ToDocument().Criteria, document.Created, DateTime.UtcNow);

        public PresetItem ToItem() =>
            new()
            {
                Id = document.Id,
                Name = document.Name,
                Description = document.Description,
                Default = document.Default,
                Created = document.Created,
                Updated = document.Updated ?? document.Created,

                Criteria = new()
                {
                    ProjectKey = document.Criteria.ProjectKey,

                    Types = document.Criteria.Types,
                    Statuses = document.Criteria.Statuses,
                    Assignees = document.Criteria.Assignees,
                    Components = document.Criteria.Components,
                    Labels = document.Criteria.Labels,

                    IncludeUnassigned = document.Criteria.IncludeUnassigned,

                    DateFilters = document.Criteria.DateFilters.ToDictionary(entry => entry.Key, entry => Enum.TryParse<Preset>(entry.Value, true, out var p) ? p : Preset.None),

                    StatusTransition = document.Criteria.StatusTransition is not null
                        ? new TransitionFilterDocument
                        {
                            Field = document.Criteria.StatusTransition.Field,
                            Preset = Enum.TryParse<Preset>(document.Criteria.StatusTransition.Preset, true, out var p) ? p : Preset.None
                        }
                        : null
                }
            };
    }
}