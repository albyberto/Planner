using Planner.Domain;

namespace Planner.Extensions;

public static class JiraExtensions
{
    extension(int seconds)
    {
        public string ToJiraTime() => ((int?)seconds).ToJiraTime();
    }

    private static string ToJiraTime(this int? seconds)
    {
        if (seconds is null or <= 0) return "0h";
        
        var s = seconds.Value;
        
        var days = s / 28800;
        s %= 28800;
        
        var hours = s / 3600;
        s %= 3600;
        
        var minutes = s / 60;

        return days switch
        {
            > 0 when hours > 0 => $"{days}d {hours}h",
            > 0 => $"{days}d",
            _ => hours switch
            {
                > 0 when minutes > 0 => $"{hours}h {minutes}m",
                > 0 => $"{hours}h",
                _ => $"{minutes}m"
            }
        };
    }

    extension(Issue issue)
    {
        public int GetOriginalEstimate() => issue.Fields.TimeTracking.OriginalEstimateSeconds ?? 0;
        public int GetTimeSpent() => issue.Fields.Worklog.Worklogs.Sum(w => w.TimeSpentSeconds ?? 0);
    }
}