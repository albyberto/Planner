using System.Text.Json.Serialization;

namespace Planner.Domain;

public class AvatarUrls
{
    [JsonPropertyName("48x48")] public required string _48x48 { get; init; }
    [JsonPropertyName("24x24")] public required string _24x24 { get; init; }
    [JsonPropertyName("16x16")] public required string _16x16 { get; init; }
    [JsonPropertyName("32x32")] public required string _32x32 { get; init; }
}