using System.Text.Json.Serialization;

namespace Planner.Domain;

public class Transition
{
    [JsonPropertyName("id")] public required string Id { get; init; }
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("to")] public required Status To { get; init; }
    [JsonPropertyName("isGlobal")] public bool? IsGlobal { get; init; }
    [JsonPropertyName("isAvailable")] public bool? IsAvailable { get; init; }
}