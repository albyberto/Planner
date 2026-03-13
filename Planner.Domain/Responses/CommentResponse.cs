using System.Text.Json.Serialization;

namespace Planner.Domain.Responses;

public record CommentRepsonse
{
    [JsonPropertyName("maxResults")] public int? MaxResults { get; init; }
    [JsonPropertyName("total")] public int? Total { get; init; }
    [JsonPropertyName("startAt")] public int? StartAt { get; init; }
    
    [JsonPropertyName("comments")] 
    public IReadOnlyList<Comment> Comments { get; init; } = [];
}