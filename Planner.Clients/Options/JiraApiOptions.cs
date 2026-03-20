using System.ComponentModel.DataAnnotations;

namespace Planner.Clients.Options;

public class JiraApiOptions
{
    public const string SectionName = nameof(JiraApiOptions);

    [Required(ErrorMessage = "The Base URL is required.")]
    [Url(ErrorMessage = "The Base URL must be a valid URL.")]
    public string BaseUrl { get; init; } = string.Empty;

    [Required(ErrorMessage = "The email address is required.")]
    [EmailAddress(ErrorMessage = "A valid email address is required.")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "The API token must be configured.")]
    public string ApiToken { get; init; } = string.Empty;
}

