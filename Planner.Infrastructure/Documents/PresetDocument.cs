namespace Planner.Infrastructure.Documents;

public record PresetDocument(Guid Id, string Name, CriteriaDocument Criteria, DateTime Created, DateTime? Updated = null);