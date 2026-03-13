using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Planner.Domain.Responses;

public class LabelResponse
{
    [JsonPropertyName("maxResults")] public required int MaxResults { get; init; }
    [JsonPropertyName("startAt")] public required int StartAt { get; init; }
    [JsonPropertyName("total")] public required int Total { get; init; }
    [JsonPropertyName("isLast")] public required bool IsLast { get; init; }
    [JsonPropertyName("values")] public ImmutableArray<string> Values { get; init; } = [];
}