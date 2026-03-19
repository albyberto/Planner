using Planner.Model.Abstract;

namespace Planner.Model;

public record ProjectModel(string Key, AvatarsModel? Avatars = null) : IAvatar
{
    public string Key { get; init; } = Key.Trim().ToUpperInvariant();
    
    public  override string ToString() => Key;
}