using Planner.Domain;
using Planner.Extensions;

namespace Planner.Model;

public record UserModel
{
    public string AccountId { get; set; }
    public string EmailAddress { get; set; }
    public string DisplayName { get; set; }
    public string _48x48 { get; set; }
    public string _24x24 { get; set; }
    public string _16x16 { get; set; }
    public string _32x32 { get; set; }

    public UserModel(User user)
    {
        AccountId = user.AccountId;
        EmailAddress = user.EmailAddress.ToLowerInvariant(); 
        DisplayName = string.IsNullOrWhiteSpace(user.DisplayName) ? user.EmailAddress : user.DisplayName.ToTitleCase();
        _16x16 = user.AvatarUrls._16x16;
        _24x24 = user.AvatarUrls._24x24;
        _32x32 = user.AvatarUrls._32x32;
        _48x48 = user.AvatarUrls._48x48;
    }
}