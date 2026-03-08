using Planner.Domain;
using Planner.Model;

namespace Planner.Extensions;

public static class GroupByExtensions
{
    extension(IEnumerable<IssueModel> issues)
    {
        public IEnumerable<(FixVersionModel Version, List<IssueModel> Issues)> GroupByFixVersion() =>
            issues
                .SelectMany(issue => issue.FixVersions is { Count: > 0 }
                    ? issue.FixVersions.Select(v => (Version: v, Issue: issue))
                    : [(Version: new FixVersionModel(new FixVersion("", "", "", "Senza Fix Version", null, null, false, false)), Issue: issue)])
                .GroupBy(tuple => tuple.Version.Name)
                .OrderBy(group => group.Key)
                .Select(group => (group.First().Version, group.Select(x => x.Issue).ToList()));
    }
}