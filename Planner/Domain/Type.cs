using System.Text.Json.Serialization;

namespace Planner.Domain;

public record Type(
    [property: JsonPropertyName("self")] string Self,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("subtask")] bool? Subtask,
    [property: JsonPropertyName("statuses")] IReadOnlyList<Status> Statuses
);