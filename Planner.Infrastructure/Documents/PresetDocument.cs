namespace Planner.Infrastructure.Documents;

public record PresetDocument(Guid Id, string Name, string Description, bool Default, CriteriaDocument Criteria, DateTime Created, DateTime? Updated = null);