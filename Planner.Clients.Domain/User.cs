using System.Text.Json.Serialization;

namespace Planner.Clients.Domain;

public class User
{
    [JsonPropertyName("accountId")] public required string AccountId { get; init; }
    [JsonPropertyName("displayName")] public required string DisplayName { get; init; }
    [JsonPropertyName("accountType")] public required string AccountType { get; init; }
    [JsonPropertyName("self")] public string? Self { get; init; }
    [JsonPropertyName("emailAddress")] public string? EmailAddress { get; init; }
    [JsonPropertyName("avatarUrls")] public AvatarUrls? AvatarUrls { get; init; }
    [JsonPropertyName("active")] public bool? Active { get; init; }
    [JsonPropertyName("timeZone")] public string? TimeZone { get; init; }
    [JsonPropertyName("locale")] public string? Locale { get; init; }
    [JsonPropertyName("appType")] public string? AppType { get; init; }
}