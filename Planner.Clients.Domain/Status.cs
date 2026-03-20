using System.Text.Json.Serialization;

namespace Planner.Clients.Domain;

public class Status
{
    [JsonPropertyName("self")] public string? Self { get; init; }
    [JsonPropertyName("id")] public required string Id { get; init; }
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("description")] public string? Description { get; init; }
    [JsonPropertyName("iconUrl")] public string? IconUrl { get; init; }
    [JsonPropertyName("statusCategory")] public required StatusCategory StatusCategory { get; init; }
}

public class StatusCategory
{
    [JsonPropertyName("self")] public string? Self { get; init; }
    [JsonPropertyName("id")] public required int Id { get; init; }
    [JsonPropertyName("key")] public required string Key { get; init; }
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("colorName")] public string? ColorName { get; init; }
}