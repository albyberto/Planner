namespace Planner.Model;

public record TimeStats(int OriginalEstimate, int TimeSpent, int? ExplicitRemaining = null)
{
    public int CalculatedRemaining => Math.Max(OriginalEstimate - TimeSpent, 0);
    public int Remaining => ExplicitRemaining ?? CalculatedRemaining;
    public int OverBudgetAmount => Math.Max(TimeSpent - OriginalEstimate, 0);
    public bool IsOverBudget => OriginalEstimate > 0 && TimeSpent > OriginalEstimate;
    public double ProgressPercent => OriginalEstimate > 0 ? Math.Min((double)TimeSpent / OriginalEstimate * 100, 100) : 0;

    public string FormattedEstimate => FormatTime(OriginalEstimate);
    public string FormattedSpent => FormatTime(TimeSpent);
    public string FormattedCalculatedRemaining => FormatTime(CalculatedRemaining);
    public string FormattedRemaining => IsOverBudget ? $"-{FormatTime(OverBudgetAmount)}" : FormatTime(Remaining);

    public static int ParseTime(string time)
    {
        if (string.IsNullOrWhiteSpace(time)) return 0;
        int seconds = 0;
        var parts = time.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            if (part.EndsWith("w") && int.TryParse(part[..^1], out int w)) seconds += w * 5 * 8 * 3600;
            else if (part.EndsWith("d") && int.TryParse(part[..^1], out int d)) seconds += d * 8 * 3600;
            else if (part.EndsWith("h") && int.TryParse(part[..^1], out int h)) seconds += h * 3600;
            else if (part.EndsWith("m") && int.TryParse(part[..^1], out int m)) seconds += m * 60;
        }
        return seconds;
    }

    private static string FormatTime(int seconds)
    {
        if (seconds <= 0) return "0h";

        var days = seconds / 28800;
        seconds %= 28800;
        var hours = seconds / 3600;
        seconds %= 3600;
        var minutes = seconds / 60;

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
}