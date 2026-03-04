using Planner.Domain;

namespace Planner.Extensions;

public static class GroupByExtensions
{
    extension(IEnumerable<Issue> issues)
    {
        public IEnumerable<(string VersionName, List<Issue> Issues)> GroupByFixVersion() =>
            issues
                .SelectMany(issue => issue.Fields.FixVersions is { Count: > 0 }
                    ? issue.Fields.FixVersions.Select(v => (Version: v.Name, Issue: issue))
                    : [(Version: "Senza Fix Version", Issue: issue)])
                .GroupBy(tuple => tuple.Version)
                .OrderBy(group => group.Key)
                .Select(group => (group.Key, group.Select(x => x.Issue).ToList()));
    }
}