using System.Collections.Immutable;

namespace Planner.Components.Shared.Filters.Model;

public static class DatePreset
{
    public enum Preset
    {
        None,
        Today,
        Yesterday,
        LastWeek,
        LastMonth,
        ThisWeek, 
        Last7Days,
        Tomorrow,
        Next7Days,
        NextWeek,
        NextMonth
    }

    public enum Category
    {
        None,
        Past,
        Present,
        Future
    }

    public record Option(Preset Value, Category Category);
    
    public static readonly ImmutableArray<Option> Options =
    [
        new(Preset.None, Category.None),

        // Past
        new(Preset.LastMonth, Category.Past),
        new(Preset.LastWeek, Category.Past),
        new(Preset.Last7Days, Category.Past),
        new(Preset.Yesterday, Category.Past),

        // Present
        new(Preset.Today, Category.Present),
        new(Preset.ThisWeek, Category.Present),

        // Future
        new(Preset.Tomorrow, Category.Future),
        new(Preset.Next7Days, Category.Future),
        new(Preset.NextWeek, Category.Future),
        new(Preset.NextMonth, Category.Future)
    ];    
}