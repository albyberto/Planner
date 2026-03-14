namespace Planner.Model;

public record ProjectModel(string Key, AvatarsModel Avatars)
{
    public string Key { get; init; } = Key.ToUpperInvariant();
}