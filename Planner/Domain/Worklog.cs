using System.Text.Json.Serialization;

namespace Planner.Domain;

public record Worklog(
    [property: JsonPropertyName("startAt")] int? StartAt,
    [property: JsonPropertyName("maxResults")] int? MaxResults,
    [property: JsonPropertyName("total")] int? Total,
    [property: JsonPropertyName("worklogs")] IReadOnlyList<Worklog2> Worklogs
);