using System.Collections.Immutable;
using MudBlazor;
using Planner.Extensions; // <-- Aggiunto per poter usare l'enum Color

namespace Planner.Components.Shared.Filters.Model;

public static class DatePreset
{
    public enum Preset
    {
        None, Today, Yesterday, LastWeek, LastMonth, ThisWeek, 
        Last7Days, Tomorrow, Next7Days, NextWeek, NextMonth
    }
    
    public record Option(Preset Value, string Name, Color IconColor);
    
    public static readonly ImmutableArray<Option> Options =
    [
        new(Preset.None, string.Empty, Color.Transparent),

        // Past (Error = Rosso/Arancio)
        new(Preset.LastMonth, $"{Preset.LastMonth.ToTitleCase()}", Color.Error),
        new(Preset.LastWeek, $"{Preset.LastWeek.ToTitleCase()}", Color.Error),
        new(Preset.Last7Days, $"{Preset.Last7Days.ToTitleCase()}", Color.Error),
        new(Preset.Yesterday, $"{Preset.Yesterday.ToTitleCase()}", Color.Error),

        // Present (Success = Verde)
        new(Preset.Today, $"{Preset.Today.ToTitleCase()}", Color.Success),
        new(Preset.ThisWeek, $"{Preset.ThisWeek.ToTitleCase()}", Color.Success),

        // Future (Info = Azzurro)
        new(Preset.Tomorrow, $"{Preset.Tomorrow.ToTitleCase()}", Color.Info),
        new(Preset.Next7Days, $"{Preset.Next7Days.ToTitleCase()}", Color.Info),
        new(Preset.NextWeek, $"{Preset.NextWeek.ToTitleCase()}", Color.Info),
        new(Preset.NextMonth, $"{Preset.NextMonth.ToTitleCase()}", Color.Info)
    ];    
}