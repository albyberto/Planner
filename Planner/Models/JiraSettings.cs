﻿namespace Planner.Models;

public class JiraSettings
{
    public const string SectionName = "Jira";

    public string BaseUrl { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ApiToken { get; set; } = string.Empty;
    public IEnumerable<string> ProjectKeys { get; set; } = [];
    public List<string> DefaultStatuses { get; set; } = [];
    public List<TeamMember> TeamMembers { get; set; } = [];
    public bool IncludeUnassignedByDefault { get; set; }
    public List<JiraPreset> Presets { get; set; } = [];
}

public class TeamMember
{
    public string Email { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = true;
}

public class JiraPreset
{
    public string Name { get; set; } = string.Empty;
    public string Jql { get; set; } = string.Empty;
}

