using Planner.Infrastructure.Domain.Enums;

namespace Planner.Components.Shared.Filters.Model;

public record DateFilterState<TField>(
    TField? Field = default,
    Preset Preset = Preset.None
);