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

    public UserModel(User? user)
    {
        if (user is null) return;
        
        AccountId = user.AccountId ?? string.Empty;
        EmailAddress = user.EmailAddress?.ToLowerInvariant() ?? string.Empty; 
        DisplayName = string.IsNullOrWhiteSpace(user.DisplayName) ? EmailAddress : user.DisplayName.ToTitleCase();
        
        if (user.AvatarUrls is not null)
        {
            _16x16 = user.AvatarUrls._16x16 ?? string.Empty;
            _24x24 = user.AvatarUrls._24x24 ?? string.Empty;
            _32x32 = user.AvatarUrls._32x32 ?? string.Empty;
            _48x48 = user.AvatarUrls._48x48 ?? string.Empty;
        }
    }
}