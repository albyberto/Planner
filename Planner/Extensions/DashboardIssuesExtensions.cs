using Planner.Model;

namespace Planner.Extensions;

public static class DashboardIssuesExtensions
{
    public static TimeStats GetTimeStats(this ICollection<DashboardIssueModel> issues) =>
        issues.Aggregate(TimeStats.Empty, (currentStats, issue) => currentStats + issue.Stats);

    // public static IEnumerable<(FixVersionModel Version, ICollection<DashboardIssueModel> Issues)> GroupByFixVersion(this ICollection<DashboardIssueModel> issues)
    // {
    //     return issues.Count == 0
    //         ? []
    //         : issues
    //         .SelectMany(issue => issue.FixVersions != null && issue.FixVersions.Any()
    //             ? issue.FixVersions.Select(v => (Version: v, Issue: issue))
    //             : [(Version: new FixVersionModel(new FixVersion("", "", "", "Senza Fix Version", null, null, false, false)), Issue: issue)])
    //         .GroupBy(tuple => tuple.Version.Name)
    //         .OrderBy(group => group.Key)
    //         .Select(group => (
    //             Version: group.First().Version, 
    //             Issues: (ICollection<DashboardIssueModel>)group.Select(x => x.Issue).ToList()
    //         ));
    // }
}