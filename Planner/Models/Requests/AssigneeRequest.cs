using System.Text.Json.Serialization;

namespace Planner.Models.Requests;

// Root myDeserializedClass = JsonSerializer.Deserialize<List<Root>>(myJsonResponse);

public record AssigneeRequest(
    [property: JsonPropertyName("self")] string Self,
    [property: JsonPropertyName("accountId")] string AccountId,
    [property: JsonPropertyName("accountType")] string AccountType,
    [property: JsonPropertyName("emailAddress")] string EmailAddress,
    [property: JsonPropertyName("avatarUrls")] AvatarUrls AvatarUrls,
    [property: JsonPropertyName("displayName")] string DisplayName,
    [property: JsonPropertyName("active")] bool? Active,
    [property: JsonPropertyName("timeZone")] string TimeZone,
    [property: JsonPropertyName("locale")] string Locale
);

