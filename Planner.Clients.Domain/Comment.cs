using System.Text.Json.Serialization;

namespace Planner.Clients.Domain;

public class Comment
{
    [JsonPropertyName("id")] public string Id { get; init; } = string.Empty;
    [JsonPropertyName("author")] public User? Author { get; init; }
    [JsonPropertyName("created")] public DateOnly? Created { get; init; }
    [JsonPropertyName("body")] public AdfDocument? Body { get; init; }
}