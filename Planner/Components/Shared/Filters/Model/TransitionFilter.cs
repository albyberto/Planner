namespace Planner.Components.Shared.Filters.Model;

public record TransitionFilter<TItem>(TItem? Item = default, DatePreset.Preset Preset = DatePreset.Preset.None);