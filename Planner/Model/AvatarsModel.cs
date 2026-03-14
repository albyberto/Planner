using Planner.Domain;

namespace Planner.Model;

public record AvatarsModel
{
    public string? _48x48 { get; init; }
    public string? _24x24 { get; init; }
    public string? _16x16 { get; init; }
    public string? _32x32 { get; init; }

    public AvatarsModel(AvatarUrls? avatarUrls)
    {
        _48x48 = avatarUrls?._48x48;
        _24x24 = avatarUrls?._24x24;
        _16x16 = avatarUrls?._16x16;
        _32x32 = avatarUrls?._32x32;
    }
}