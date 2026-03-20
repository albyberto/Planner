using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Planner.Clients.Domain.Responses;

public class ProjectResponse
{
    [JsonPropertyName("self")] public string Self { get; init; } = string.Empty;
    [JsonPropertyName("nextPage")] public string? NextPage { get; init; }
    [JsonPropertyName("maxResults")] public int MaxResults { get; init; }
    [JsonPropertyName("startAt")] public int StartAt { get; init; }
    [JsonPropertyName("total")] public int Total { get; init; }
    [JsonPropertyName("isLast")] public bool IsLast { get; init; }
    [JsonPropertyName("values")] public ImmutableArray<Project> Projects { get; init; } = [];
}