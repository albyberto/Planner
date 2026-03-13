using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Planner.Domain.Responses;

public class IssuesResponse
{
    [JsonPropertyName("issues")] public ImmutableArray<Issue> Issues { get; init; } = [];
    [JsonPropertyName("isLast")] public bool IsLast { get; init; }
    [JsonPropertyName("nextPageToken")] public string? NextPageToken { get; init; }
}