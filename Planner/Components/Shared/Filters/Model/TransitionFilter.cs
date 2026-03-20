using Planner.Infrastructure.Domain.Enums;

namespace Planner.Components.Shared.Filters.Model;

public record TransitionFilter<TItem>(TItem? Item = default, Preset Preset = Preset.None);