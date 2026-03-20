using Planner.Infrastructure.Domain.Enums;

namespace Planner.Infrastructure.Domain;

public record TransitionFilterDocument
{
    public required string Field { get; init; }
    public required Preset Preset { get; init; }
}