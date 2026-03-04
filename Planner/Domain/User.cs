using System.Globalization;
using System.Text.Json.Serialization;

namespace Planner.Domain;

public record User(
    [property: JsonPropertyName("self")] string Self,
    [property: JsonPropertyName("accountId")] string AccountId,
    [property: JsonPropertyName("accountType")] string AccountType,
    [property: JsonPropertyName("emailAddress")] string EmailAddress,
    [property: JsonPropertyName("avatarUrls")] AvatarUrls AvatarUrls,
    [property: JsonPropertyName("displayName")] string DisplayName,
    [property: JsonPropertyName("active")] bool? Active,
    [property: JsonPropertyName("timeZone")] string TimeZone,
    [property: JsonPropertyName("locale")] string Locale
)
{
    [JsonIgnore]
    public string FormattedDisplayName => string.IsNullOrWhiteSpace(DisplayName) 
        ? DisplayName 
        : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DisplayName.ToLower());
}

