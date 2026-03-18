namespace Planner.Components.Shared.Filters.Model;

public record DateFilterState<TField>(
    TField? Field = default,
    DatePreset.Preset Preset = DatePreset.Preset.None
);