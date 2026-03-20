using Planner.Domain;
using Planner.Extensions;
using Planner.Model.Abstract;

namespace Planner.Model;

public record UserModel : IAvatar
{
    public string AccountId { get; init; }
    public string EmailAddress { get; init; }
    public string DisplayName { get; init; }
    public AvatarsModel Avatars { get; init; }

    public UserModel(User user)
    {
        AccountId = user.AccountId;
        EmailAddress = user.EmailAddress?.ToLowerInvariant() ?? string.Empty; 
        DisplayName = string.IsNullOrWhiteSpace(user.DisplayName) ? EmailAddress : user.DisplayName.ToTitleCase();
        Avatars = new(user.AvatarUrls);
    }
    
    public override string ToString() => DisplayName;
}