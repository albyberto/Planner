using System.Text.Json.Serialization;
using Planner.Extensions;

namespace Planner.Domain;

public record Issue(
    [property: JsonPropertyName("expand")] string Expand,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("self")] string Self,
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("fields")] Fields Fields
)
{
    [JsonIgnore] public int OriginalEstimate => Fields.TimeTracking.OriginalEstimateSeconds ?? 0;
    [JsonIgnore] public int TimeSpent => Fields.Worklog.Worklogs.Sum(w => w.TimeSpentSeconds) ?? 0;

    [JsonIgnore] public TimeStats Stats => new TimeStats(OriginalEstimate, TimeSpent);
}