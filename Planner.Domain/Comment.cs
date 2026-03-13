using System.Text.Json.Serialization;

namespace Planner.Domain;

public class Comment
{
    [JsonPropertyName("id")] public required string Id { get; init; }
    [JsonPropertyName("author")] public required User Author { get; init; }
    [JsonPropertyName("created")] public required string Created { get; init; }
    [JsonPropertyName("body")] public required AdfDocument Body { get; init; }
}