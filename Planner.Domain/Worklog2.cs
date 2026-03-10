using System.Text.Json.Serialization;

namespace Planner.Domain;

public record Worklog2(
    [property: JsonPropertyName("self")] string Self,
    [property: JsonPropertyName("author")] User Author,
    [property: JsonPropertyName("updateAuthor")] User UpdateAuthor,
    [property: JsonPropertyName("comment")] Comment Comment,
    [property: JsonPropertyName("created"), JsonConverter(typeof(JiraDateOnlyConverter))] DateOnly? Created,
    [property: JsonPropertyName("updated"), JsonConverter(typeof(JiraDateOnlyConverter))] DateOnly? Updated,
    [property: JsonPropertyName("started"), JsonConverter(typeof(JiraDateOnlyConverter))] DateOnly? Started,
    [property: JsonPropertyName("timeSpent")] string TimeSpent,
    [property: JsonPropertyName("timeSpentSeconds")] int? TimeSpentSeconds,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("issueId")] string IssueId
);