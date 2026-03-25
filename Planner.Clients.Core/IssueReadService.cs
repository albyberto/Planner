using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Planner.Clients.Domain;

namespace Planner.Clients.Core;

public class IssueReadService(JiraReadClient jiraReadClient, ILogger<IssueReadService> logger)
{
    private static readonly string[] DashboardFields = [
        "summary", "status", "assignee", "fixVersions",
        "created", "updated", "issuetype", "components",
        "labels", "timetracking", "worklog", "customfield_10117", "project", "comment",
        "parent", "customfield_10014"
    ];
    
    private static readonly string[] DashboardExpands = [
        "transitions"
    ];
    
    public async Task<ImmutableHashSet<Issue>> GetDashboardIssuesAsync(string jql, CancellationToken cancellationToken = default)
    {
        try
        {
            return await jiraReadClient.GetIssuesAsync(jql, DashboardFields, DashboardExpands, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error fetching standard issues with JQL: {Jql}", jql);
            throw;
        }
    }
}