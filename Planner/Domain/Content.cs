using System.Text.Json.Serialization;

namespace Planner.Domain;

public record Content(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("content")] IReadOnlyList<Content> Contents,
    [property: JsonPropertyName("text")] string Text
);