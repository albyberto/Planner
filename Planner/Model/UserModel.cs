using Planner.Domain;
using Planner.Extensions;

namespace Planner.Model;

public record UserModel
{
    public string AccountId { get; init; }
    public string EmailAddress { get; init; }
    public string DisplayName { get; init; }
    public AvatarsModel Avatars { get; init; }

    public UserModel(User user)
    {
        AccountId = user.AccountId;
        EmailAddress = user.EmailAddress.ToLowerInvariant(); 
        DisplayName = string.IsNullOrWhiteSpace(user.DisplayName) ? EmailAddress : user.DisplayName.ToTitleCase();
        Avatars = new(user.AvatarUrls);
    }
}