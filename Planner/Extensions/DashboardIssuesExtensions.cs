using Planner.Model;

namespace Planner.Extensions;

public static class DashboardIssuesExtensions
{
    public static TimeStats GetTimeStats(this IEnumerable<IHasStats> issues) =>
        issues.Aggregate(TimeStats.Empty, (currentStats, issue) => currentStats + issue.Stats);

    public static IEnumerable<(FixVersionModel Version, IEnumerable<T> Issues)> GroupByFixVersion<T>(this IEnumerable<T> issues) where T : IHasFixVersions
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
                Issues: (IEnumerable<T>)group.Select(x => x.Issue).ToList()
            ));
    }

    public static IEnumerable<(EpicModel Epic, IEnumerable<T> Issues)> GroupByEpic<T>(this IEnumerable<T> issues) where T : IHasEpic
    {
        var issueList = issues.ToList();
        return issueList.Count == 0
            ? []
            : issueList
            .Select(issue => issue.Epic != null ? (Epic: issue.Epic, Issue: issue) : (Epic: new EpicModel("No Epic", "Unassigned Epic"), Issue: issue))
            .GroupBy(tuple => tuple.Epic.Summary)
            .OrderBy(group => group.Key)
            .Select(group => (
                Epic: group.First().Epic,
                Issues: (IEnumerable<T>)group.Select(x => x.Issue).ToList()
            ));
    }

    public static IEnumerable<(FixVersionModel Version, IEnumerable<(EpicModel Epic, IEnumerable<T> Issues)> Epics)> GroupByVersionAndEpic<T>(this IEnumerable<T> issues) where T : IHasFixVersions, IHasEpic
    {
        var versionGroups = issues.GroupByFixVersion();
        return versionGroups.Select(vg => (
            vg.Version,
            Epics: vg.Issues.GroupByEpic()
        ));
    }
}