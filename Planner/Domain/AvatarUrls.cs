using System.Text.Json.Serialization;

namespace Planner.Domain;

public record AvatarUrls(
    [property: JsonPropertyName("48x48")] string _48x48,
    [property: JsonPropertyName("24x24")] string _24x24,
    [property: JsonPropertyName("16x16")] string _16x16,
    [property: JsonPropertyName("32x32")] string _32x32
);