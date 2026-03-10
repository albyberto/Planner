using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Planner.Domain;

public record IssueType(
    [property: JsonPropertyName("self")] string Self,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("iconUrl")] string? IconUrl,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("subtask")] bool? Subtask,
    [property: JsonPropertyName("avatarId")] int? AvatarId,
    [property: JsonPropertyName("hierarchyLevel")] int? HierarchyLevel,
    [property: JsonPropertyName("statuses")] ImmutableList<Status> Statuses
);