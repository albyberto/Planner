using System.Text.Json.Serialization;

namespace Planner.Domain;

public record Project(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("lead")] User? Lead
);