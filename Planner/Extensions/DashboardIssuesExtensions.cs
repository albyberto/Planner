using Planner.Model;

namespace Planner.Extensions;

public static class DashboardIssuesExtensions
{
    public static TimeStats GetTimeStats(this IEnumerable<IIssue> issues) =>
        issues.Aggregate(TimeStats.Empty, (currentStats, issue) => currentStats + issue.Stats);

    public static IEnumerable<(FixVersionModel Version, IEnumerable<IIssue> Issues)> GroupByFixVersion(this IEnumerable<IIssue> issues)
    {
        var issueList = issues.ToList();
        return issueList.Count == 0
            ? []
            : issueList
            .SelectMany(issue => issue.FixVersions != null && issue.FixVersions.Any()
                ? issue.FixVersions.Select(v => (Version: v, Issue: issue))
                : [(Version: FixVersionModel.Unassigned, Issue: issue)])
            .GroupBy(tuple => tuple.Version.Name)
            .OrderBy(group => group.Key)
            .Select(group => (
                Version: group.First().Version, 
                Issues: (IEnumerable<IIssue>)group.Select(x => x.Issue).ToList()
            ));
    }
}