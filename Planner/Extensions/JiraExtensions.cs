using Planner.Domain;

namespace Planner.Extensions;

public static class JiraExtensions
{
    extension(ICollection<Issue> issues)
    {
        public TimeStats GetTimeStats() =>
            new(issues.Sum(i => i.OriginalEstimate), issues.Sum(i => i.TimeSpent));
    }
}