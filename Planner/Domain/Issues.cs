using System.Text.Json.Serialization;

namespace Planner.Domain;

public record Issues(
    [property: JsonPropertyName("issues")] IReadOnlyList<Issue> List,
    [property: JsonPropertyName("isLast")] bool? IsLast,
    [property: JsonPropertyName("nextPageToken")] string? NextPageToken
);