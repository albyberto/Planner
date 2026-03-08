using System.Text.Json;
using System.Text.Json.Serialization;

namespace Planner.Domain;

public record IssueComment(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("author")] User Author,
    [property: JsonPropertyName("body")] JsonDocument Body,
    [property: JsonPropertyName("created"), JsonConverter(typeof(JiraDateTimeConverter))] DateTime? Created,
    [property: JsonPropertyName("updated"), JsonConverter(typeof(JiraDateTimeConverter))] DateTime? Updated
);
