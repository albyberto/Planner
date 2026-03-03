using System.Collections.Immutable;

namespace Planner.Model;

public record Filter(ImmutableList<string>? Assignees = null, ImmutableList<string>? Statuses = null)
{
    public ImmutableList<string> Assignees { get; init; } = Assignees ?? [];
    public ImmutableList<string> Statuses { get; init; } = Statuses ?? [];
}
