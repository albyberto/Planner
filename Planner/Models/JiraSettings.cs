using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Planner.Models;

public class JiraSettings
{
    public const string SectionName = "Jira";

    [Required(ErrorMessage = "The Base URL is required.")]
    [Url(ErrorMessage = "The Base URL must be a valid URL.")]
    public string BaseUrl { get; init; } = string.Empty;

    [Required(ErrorMessage = "The email address is required.")]
    [EmailAddress(ErrorMessage = "A valid email address is required.")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "The API token must be configured.")]
    public string ApiToken { get; init; } = string.Empty;

    [Required(ErrorMessage = "The project keys list is required.")]
    [MinLength(1, ErrorMessage = "At least one project key must be configured.")]
    public List<string> ProjectKeys { get; init; } = [];

    public List<string> DefaultStatuses { get; init; } = [];
    
    [Required(ErrorMessage = "The team members list is required.")]
    [MinLength(1, ErrorMessage = "At least one team member must be configured.")]
    public List<TeamMember> TeamMembers { get; init; } = [];
    
    public List<JiraPreset> Presets { get; init; } = [];
    
    public bool IncludeUnassignedByDefault { get; init; } = true;
}

public class TeamMember
{
    [Required(ErrorMessage = "The team member's email address is required.")]
    [EmailAddress(ErrorMessage = "A valid email address is required for the team member.")]
    public string Email { get; init; } = string.Empty;
    
    public bool IsDefault { get; init; } = true;
}

public class JiraPreset
{
    [Required(ErrorMessage = "The preset name is required.")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "The JQL query is required.")]
    public string Jql { get; init; } = string.Empty;
}