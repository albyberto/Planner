using System.Text.Json.Serialization;

namespace Planner.Domain;

public record Comment(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("version")] int? Version,
    [property: JsonPropertyName("content")] IReadOnlyList<Content> Content
);