using System.Text.Json.Serialization;

namespace Planner.Domain;

public record TimeTracking(
    [property: JsonPropertyName("originalEstimate")] string OriginalEstimate,
    [property: JsonPropertyName("timeSpent")] string TimeSpent,
    [property: JsonPropertyName("originalEstimateSeconds")] int? OriginalEstimateSeconds,
    [property: JsonPropertyName("timeSpentSeconds")] int? TimeSpentSeconds,
    [property: JsonPropertyName("remainingEstimateSeconds")] int? RemainingEstimateSeconds
);