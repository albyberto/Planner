using System.Text.Json.Serialization;

namespace Planner.Domain;

public record TimeTracking
{
    [JsonPropertyName("originalEstimate")] public string? OriginalEstimate { get; init; }

    [JsonPropertyName("remainingEstimate")] public string? RemainingEstimate { get; init; }

    [JsonPropertyName("timeSpent")] public string? TimeSpent { get; init; }

    [JsonPropertyName("originalEstimateSeconds")] public int? OriginalEstimateSeconds { get; init; }

    [JsonPropertyName("remainingEstimateSeconds")] public int? RemainingEstimateSeconds { get; init; }

    [JsonPropertyName("timeSpentSeconds")] public int? TimeSpentSeconds { get; init; }
}