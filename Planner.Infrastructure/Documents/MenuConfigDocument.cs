using System.Collections.Immutable;

namespace Planner.Infrastructure.Documents;

public record MenuConfigDocument(
    Guid Id, 
    string ProjectKey, 
    ImmutableArray<string> Assignees, 
    DateTime Created, 
    DateTime? Updated = null);
