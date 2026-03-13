using System.Text.Json.Serialization;

namespace Planner.Domain;

public record Worklog
{
    [JsonPropertyName("startAt")] public int? StartAt { get; init; }
    [JsonPropertyName("maxResults")] public int? MaxResults { get; init; }
    [JsonPropertyName("total")] public int? Total { get; init; }
    [JsonPropertyName("worklogs")] public IReadOnlyList<WorklogItem> Worklogs { get; init; } = [];
}

public record WorklogItem
{
    [JsonPropertyName("id")] public required string Id { get; init; }

    [JsonPropertyName("issueId")] public required string IssueId { get; init; }

    [JsonPropertyName("self")] public string? Self { get; init; }

    [JsonPropertyName("author")] public User? Author { get; init; }

    [JsonPropertyName("updateAuthor")] public User? UpdateAuthor { get; init; }

    [JsonPropertyName("comment")] public AdfDocument? Comment { get; init; }

    [JsonPropertyName("created")]
    [JsonConverter(typeof(JiraDateOnlyConverter))]
    public DateOnly? Created { get; init; }

    [JsonPropertyName("updated")]
    [JsonConverter(typeof(JiraDateOnlyConverter))]
    public DateOnly? Updated { get; init; }

    [JsonPropertyName("started")]
    [JsonConverter(typeof(JiraDateOnlyConverter))]
    public DateOnly? Started { get; init; }

    [JsonPropertyName("timeSpent")] public string? TimeSpent { get; init; }

    [JsonPropertyName("timeSpentSeconds")] public int? TimeSpentSeconds { get; init; }
}