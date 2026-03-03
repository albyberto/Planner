using System.Text.Json.Serialization;

namespace Planner.Models;

public record StatusCategory(
    [property: JsonPropertyName("self")] string Self,
    [property: JsonPropertyName("id")] int? Id,
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("colorName")] string ColorName,
    [property: JsonPropertyName("name")] string Name
);