namespace Planner.Model;

public record ProjectModel(string Key, AvatarsModel? Avatars = null)
{
    public string Key { get; init; } = Key.ToUpperInvariant();
}