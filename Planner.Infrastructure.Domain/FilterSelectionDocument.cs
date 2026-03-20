using System.Collections.Immutable;

namespace Planner.Infrastructure.Domain;

public record FilterSelectionDocument
{
    public ImmutableHashSet<string> Included { get; init; } = [];
    public ImmutableHashSet<string> Excluded { get; init; } = [];
}