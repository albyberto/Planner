using System.Text.Json.Serialization;

namespace Planner.Domain;

public record Status(
    [property: JsonPropertyName("self")] string Self,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("iconUrl")] string IconUrl,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("untranslatedName")] string UntranslatedName,
    [property: JsonPropertyName("id")] string Id, 
    [property: JsonPropertyName("statusCategory")] StatusCategory StatusCategory
);
