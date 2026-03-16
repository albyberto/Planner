// using Microsoft.AspNetCore.Components;
// using Planner.Model;
//
// namespace Planner.Components.Shared.Filters.Controls;
//
// public partial class TimeFilterControl
// {
//     [Parameter] public TimeFilter? Value { get; set; }
//     [Parameter] public EventCallback<TimeFilter?> ValueChanged { get; set; }
//     [Parameter] public bool Disabled { get; set; }
//     [Parameter] public IEnumerable<StatusModel> Statuses { get; set; } = [];
//
//     private async Task UpdateValue(Func<TimeFilter, TimeFilter> updateAction)
//     {
//         var current = Value ?? new TimeFilter(
//             Target: TimeFilterTarget.CreatedDate,
//             Preset: RelativeTimePreset.None,
//             Range: new(null, null),
//             StatusName: null);
//
//         var updated = updateAction(current);
//         await ValueChanged.InvokeAsync(updated);
//     }
//
//     private Task HandleTargetChanged(TimeFilterTarget target) => UpdateValue(v => v with { Target = target });
//     
//     private async Task HandlePresetChanged(RelativeTimePreset preset)
//     {
//         if (preset == RelativeTimePreset.None) await ValueChanged.InvokeAsync(null);
//         else await UpdateValue(v => v with { Preset = preset });
//     }
//
//     private Task HandleStatusChanged(string name) => UpdateValue(v => v with { StatusName = name });
//
//     private Task HandleFromChanged(DateTime? date) => UpdateValue(v => 
//         v with { Preset = RelativeTimePreset.CustomRange, Range = v.Range with { From = date.HasValue ? DateOnly.FromDateTime(date.Value) : null } });
//
//     private Task HandleToChanged(DateTime? date) => UpdateValue(v => 
//         v with { Preset = RelativeTimePreset.CustomRange, Range = v.Range with { To = date.HasValue ? DateOnly.FromDateTime(date.Value) : null } });
//
//     private static string GetPresetLabel(RelativeTimePreset preset) => preset switch {
//         RelativeTimePreset.None => "Sempre (Nessun filtro)",
//         RelativeTimePreset.Last7Days => "Ultimi 7 giorni",
//         RelativeTimePreset.Last30Days => "Ultimi 30 giorni",
//         RelativeTimePreset.ThisWeek => "Questa settimana",
//         RelativeTimePreset.LastWeek => "Settimana scorsa",
//         RelativeTimePreset.ThisMonth => "Questo mese",
//         RelativeTimePreset.LastMonth => "Mese scorso",
//         RelativeTimePreset.CustomRange => "Intervallo personalizzato",
//         _ => preset.ToString()
//     };
// }