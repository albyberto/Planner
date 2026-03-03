using System.Text.Json.Serialization;

namespace Planner.Domain;

public record FixVersion(
    [property: JsonPropertyName("self")] string Self,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("archived")] bool? Archived,
    [property: JsonPropertyName("released")] bool? Released
);