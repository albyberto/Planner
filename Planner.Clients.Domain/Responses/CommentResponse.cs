using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Planner.Clients.Domain.Responses;

public class CommentResponse
{
    [JsonPropertyName("maxResults")] public int? MaxResults { get; init; }
    [JsonPropertyName("total")] public int? Total { get; init; }
    [JsonPropertyName("startAt")] public int? StartAt { get; init; }
    [JsonPropertyName("comments")] public ImmutableArray<Comment> Comments { get; init; } = [];
}