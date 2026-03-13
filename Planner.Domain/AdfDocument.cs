using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Planner.Domain;

public class AdfDocument
{
    [JsonPropertyName("type")] public string? Type { get; init; }
    [JsonPropertyName("version")] public int? Version { get; init; }
    [JsonPropertyName("content")] public ImmutableArray<Content> ContentList { get; init; } = [];
}

public class Content
{
    [JsonPropertyName("type")] public string? Type { get; init; }
    [JsonPropertyName("text")] public string? Text { get; init; }
    [JsonPropertyName("content")] public ImmutableArray<Content> SubContent { get; init; } = [];
    [JsonPropertyName("attrs")] public Attrs? Attributes { get; init; }
}

public class Attrs
{
    [JsonPropertyName("id")] public string? Id { get; init; }
    [JsonPropertyName("text")] public string? Text { get; init; }
    [JsonPropertyName("accessLevel")] public string? AccessLevel { get; init; }
    [JsonPropertyName("localId")] public string? LocalId { get; init; }
}