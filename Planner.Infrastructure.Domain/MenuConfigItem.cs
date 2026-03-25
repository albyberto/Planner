using System.Collections.Immutable;

namespace Planner.Infrastructure.Domain;

public record MenuConfigItem
{
    public Guid Id { get; init; }
    public string ProjectKey { get; init; } = string.Empty;
    public ImmutableArray<string> Assignees { get; init; } = [];
    public DateTime Created { get; init; }
    public DateTime Updated { get; init; }
    
    public override string ToString() => ProjectKey;
}
