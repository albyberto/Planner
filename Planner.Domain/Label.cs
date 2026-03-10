using System.Text.Json.Serialization;

namespace Planner.Domain;

public record Label(
    [property: JsonPropertyName("maxResults")] int MaxResults,
    [property: JsonPropertyName("startAt")] int StartAt,
    [property: JsonPropertyName("total")] int Total,
    [property: JsonPropertyName("isLast")] bool IsLast,
    [property: JsonPropertyName("values")] List<string> Values
);