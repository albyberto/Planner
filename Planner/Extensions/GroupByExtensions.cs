using Planner.Domain;
using Planner.Model;

namespace Planner.Extensions;

public static class GroupByExtensions
{
    extension(IEnumerable<IssueModel> issues)
    {
        public IEnumerable<(string VersionName, List<IssueModel> Issues)> GroupByFixVersion() =>
            issues
                .SelectMany(issue => issue.FixVersions is { Count: > 0 }
                    ? issue.FixVersions.Select(v => (Version: v.Name, Issue: issue))
                    : [(Version: "Senza Fix Version", Issue: issue)])
                .GroupBy(tuple => tuple.Version)
                .OrderBy(group => group.Key)
                .Select(group => (group.Key, group.Select(x => x.Issue).ToList()));
    }
}