using System.Text.Json.Serialization;

namespace Planner.Models.Requests;

// StatusRequest myDeserializedClass = JsonSerializer.Deserialize<List<StatusRequest>>(myJsonResponse);

public record StatusRequest(
    [property: JsonPropertyName("self")] string Self,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("subtask")] bool? Subtask,
    [property: JsonPropertyName("statuses")] IReadOnlyList<Status> Statuses
);