using JQLBuilder;
using JQLBuilder.Types.JqlTypes;
using Microsoft.Extensions.Options;
using Planner.Model;
using Planner.Models;
using JqlFilter = JQLBuilder.Render.JqlFilter;

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

        query = AppendAssigneeClause(query, filter);

        return $"{query} ORDER BY updated DESC";
    }

    private static JqlFilter AppendAssigneeClause(JqlFilter query, Filter filter)
    {
        var hasAssignees = filter.Assignees.Count > 0;

        switch (hasAssignees)
        {
            case true when filter.IncludeUnassigned:
            {
                var assigneeValues = filter.Assignees.Select(a => (JqlHistoricalJqlUser)a).ToArray();
                return query.And(f => f.User.Assignee.In(assigneeValues) | f.User.Assignee.Is());
            }
            case true:
            {
                var assigneeValues = filter.Assignees.Select(a => (JqlHistoricalJqlUser)a).ToArray();
                return query.And(f => f.User.Assignee.In(assigneeValues));
            }
        }

        return filter.IncludeUnassigned 
            ? query.And(f => f.User.Assignee.Is()) 
            : query;
    }
}