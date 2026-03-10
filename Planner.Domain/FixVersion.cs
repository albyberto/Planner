using System.Text.Json.Serialization;

namespace Planner.Domain;

public record FixVersion(
    [property: JsonPropertyName("self")] string Self,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("startDate")] DateOnly? StartDate,
    [property: JsonPropertyName("releaseDate")] DateOnly? ReleaseDate,
    [property: JsonPropertyName("archived")] bool? Archived,
    [property: JsonPropertyName("released")] bool? Released
);