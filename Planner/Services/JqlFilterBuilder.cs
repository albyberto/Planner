using JQLBuilder;
using JQLBuilder.Types.JqlTypes;
using Microsoft.Extensions.Options;
using Planner.Model;
using Planner.Models;

namespace Planner.Services;

public class JqlFilterBuilder(IOptions<JiraQueryOptions> options)
{
    private readonly JiraQueryOptions _settings = options.Value;

    public string BuildDashboardQuery(Filter filter)
    {
        var projectKeys = _settings.ProjectKeys.Select(key => (JqlProject)key).ToArray();
        
        var query = JqlBuilder.Query.Where(f => f.Project.In(projectKeys));

        if (filter.Statuses.Count > 0)
        {
            var statusValues = filter.Statuses.Select(s => (JqlStatus)s).ToArray();
            query = query.And(f => f.Status.In(statusValues));
        }

        if (filter.Assignees.Count > 0)
        {
            var assigneeValues = filter.Assignees.Select(a => (JqlHistoricalJqlUser)a).ToArray(); 
            query = query.And(f => f.User.Assignee.In(assigneeValues));
        }

        return $"{query} ORDER BY updated DESC";
    }
}