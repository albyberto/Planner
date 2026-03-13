using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Planner.Domain;

public class Type
{
    [JsonPropertyName("self")] public string? Self { get; init; }
    [JsonPropertyName("id")] public required string Id { get; init; }
    [JsonPropertyName("description")] public string? Description { get; init; }
    [JsonPropertyName("iconUrl")] public string? IconUrl { get; init; }
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("subtask")] public bool? Subtask { get; init; }
    [JsonPropertyName("avatarId")] public int? AvatarId { get; init; }
    [JsonPropertyName("hierarchyLevel")] public int? HierarchyLevel { get; init; }
    [JsonPropertyName("statuses")] public ImmutableArray<Status> Statuses { get; init; } = [];
}