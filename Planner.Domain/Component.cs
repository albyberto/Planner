using System.Text.Json.Serialization;

namespace Planner.Domain;

public record Component(
    [property: JsonPropertyName("self")] string Self,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name
);