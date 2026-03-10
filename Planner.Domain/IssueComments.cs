using System.Text.Json.Serialization;

namespace Planner.Domain;

public record IssueComments(
    [property: JsonPropertyName("comments")] IReadOnlyList<IssueComment> Comments,
    [property: JsonPropertyName("maxResults")] int MaxResults,
    [property: JsonPropertyName("total")] int Total,
    [property: JsonPropertyName("startAt")] int StartAt
);
