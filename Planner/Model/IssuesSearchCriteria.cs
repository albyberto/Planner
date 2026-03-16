namespace Planner.Model;

public record FilterValue(string Value, bool IsExcluded = false);

public enum TimeFilterTarget
{
    CreatedDate,
    ResolvedDate,
    StatusTransitionDate
}

public enum RelativeTimePreset
{
    None,
    Last7Days,
    Last30Days,
    ThisWeek,
    ThisMonth,
    LastWeek,
    LastMonth,
    CustomRange
}

public record TimeRange(DateOnly? From, DateOnly? To);

public record TimeFilter(TimeFilterTarget Target, RelativeTimePreset Preset, TimeRange Range, string? StatusName = null);

public record IssuesSearchCriteria(
    string? ProjectKey = null,
    HashSet<FilterValue>? Assignees = null,
    HashSet<FilterValue>? Statuses = null,
    HashSet<FilterValue>? Components = null,
    HashSet<FilterValue>? Labels = null,
    HashSet<FilterValue>? Types = null,
    bool IncludeUnassigned = false,
    TimeFilter? TimeFilter = null)
{
    public HashSet<FilterValue> Assignees { get; init; } = Assignees ?? [];
    public HashSet<FilterValue> Statuses { get; init; } = Statuses ?? [];
    public HashSet<FilterValue> Components { get; init; } = Components ?? [];
    public HashSet<FilterValue> Labels { get; init; } = Labels ?? [];
    public HashSet<FilterValue> Types { get; init; } = Types ?? [];

    public static IssuesSearchCriteria Create(string projectKey) => new(ProjectKey: projectKey);
    public static IssuesSearchCriteria Empty => new();
}