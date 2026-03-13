using System.Text.Json.Serialization;

namespace Planner.Domain;

public class ProjectCategory
{
    [JsonPropertyName("id")] public required string Id { get; init; }
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("self")] public string? Self { get; init; }
    [JsonPropertyName("description")] public string? Description { get; init; }
}

public class Project
{
    [JsonPropertyName("id")] public required string Id { get; init; }
    [JsonPropertyName("key")] public required string Key { get; init; }
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("self")] public string? Self { get; init; }
    [JsonPropertyName("projectTypeKey")] public required string ProjectTypeKey { get; init; }
    [JsonPropertyName("simplified")] public bool? Simplified { get; init; }
    [JsonPropertyName("avatarUrls")] public AvatarUrls? AvatarUrls { get; init; }
    [JsonPropertyName("projectCategory")] public ProjectCategory? ProjectCategory { get; init; }
    [JsonPropertyName("expand")] public string? Expand { get; init; }
    [JsonPropertyName("style")] public string? Style { get; init; }
    [JsonPropertyName("isPrivate")] public bool? IsPrivate { get; init; }
    [JsonPropertyName("properties")] public Dictionary<string, object>? Properties { get; init; }
}