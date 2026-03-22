using System.Collections.Immutable;
using Planner.Clients.Core;
using Planner.Mappers;
using Planner.Model;

namespace Planner.Facades;

public class DashboardFacade(IssueReadService issueService)
{
    public async Task<ImmutableArray<DashboardIssueModel>> LoadDashboardIssuesAsync(string jql, CancellationToken cancellationToken = default)
    {
        var issues = await issueService.GetDashboardIssuesAsync(jql, cancellationToken);

        return [..issues.Select(issue => issue.ToDashboardModel())];
    }
}