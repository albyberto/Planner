using System.ComponentModel.DataAnnotations;

namespace Planner.Models;

public class JiraQuerySettings
{
    public const string SectionName = "JiraQuery";

    [Required(ErrorMessage = "The project keys list is required.")]
    [MinLength(1, ErrorMessage = "At least one project key must be configured.")]
    public List<string> ProjectKeys { get; init; } = [];
}

