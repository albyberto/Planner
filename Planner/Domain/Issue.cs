// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);

using System.Text.Json.Serialization;

namespace Planner.Domain;

public record Issue(
    [property: JsonPropertyName("expand")] string Expand,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("self")] string Self,
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("fields")] Fields Fields
);