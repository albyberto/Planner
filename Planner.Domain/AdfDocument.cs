using System.Text.Json.Serialization;

namespace Planner.Domain;

// Invece di chiamarlo Comment, chiamiamolo AdfDocument. È chiarissimo a cosa serve!
public record AdfDocument
{
    [JsonPropertyName("type")] public string? Type { get; init; }
    [JsonPropertyName("version")] public int? Version { get; init; }
    [JsonPropertyName("content")] public IReadOnlyList<Content> ContentList { get; init; } = [];
}

public record Content
{
    [JsonPropertyName("type")] public string? Type { get; init; }
    [JsonPropertyName("text")] public string? Text { get; init; }
    [JsonPropertyName("content")] public IReadOnlyList<Content> SubContent { get; init; } = [];
    [JsonPropertyName("attrs")] public Attrs? Attributes { get; init; }
}

public record Attrs
{
    [JsonPropertyName("id")] public string? Id { get; init; }
    [JsonPropertyName("text")] public string? Text { get; init; }
    [JsonPropertyName("accessLevel")] public string? AccessLevel { get; init; }
    [JsonPropertyName("localId")] public string? LocalId { get; init; }
}