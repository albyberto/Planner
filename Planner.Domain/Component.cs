using System.Text.Json.Serialization;

namespace Planner.Domain;

public class Component
{
    [JsonPropertyName("id")] public required string Id { get; init; }
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("self")] public string? Self { get; init; }
    [JsonPropertyName("assigneeType")] public string? AssigneeType { get; init; }
    [JsonPropertyName("realAssigneeType")] public string? RealAssigneeType { get; init; }
    [JsonPropertyName("isAssigneeTypeValid")] public bool? IsAssigneeTypeValid { get; init; }
    [JsonPropertyName("project")] public string? ProjectKey { get; init; }
    [JsonPropertyName("projectId")] public long? ProjectId { get; init; }
}