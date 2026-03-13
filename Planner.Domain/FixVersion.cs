using System.Text.Json.Serialization;

namespace Planner.Domain;

public class FixVersion
{
    [JsonPropertyName("id")] public required string Id { get; init; }
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("self")] public string? Self { get; init; }
    [JsonPropertyName("description")] public string? Description { get; init; }
    [JsonPropertyName("startDate")] public DateOnly? StartDate { get; init; }
    [JsonPropertyName("releaseDate")] public DateOnly? ReleaseDate { get; init; }
    [JsonPropertyName("archived")] public bool? Archived { get; init; }
    [JsonPropertyName("released")] public bool? Released { get; init; }
}