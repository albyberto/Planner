using Planner.Model;

namespace Planner.Extensions;

public static class JiraExtensions
{
    extension(ICollection<IssueModel> issues)
    {
        public TimeStats GetTimeStats() =>
            new(issues.Sum(i => i.Stats.OriginalEstimate), issues.Sum(i => i.Stats.TimeSpent));
    }
}