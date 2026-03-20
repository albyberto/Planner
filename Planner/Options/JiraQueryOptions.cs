using System.ComponentModel.DataAnnotations;

namespace Planner.Options;

public class JiraQueryOptions
{
    public const string SectionName = "JiraQuery";

    [Required(ErrorMessage = "The project keys list is required.")]
    [MinLength(1, ErrorMessage = "At least one project key must be configured.")]
    public List<string> ProjectKeys { get; init; } = [];
}

