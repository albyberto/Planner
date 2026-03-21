namespace Planner.Clients.Core.Options;

public class CacheOptions
{
    public const string SectionName = nameof(CacheOptions);

    public TimeSpan Projects { get; init; } = TimeSpan.FromHours(18);
    public TimeSpan Types { get; init; } = TimeSpan.FromHours(18);
    public TimeSpan Assignees { get; init; } = TimeSpan.FromHours(4);
    public TimeSpan Components { get; init; } = TimeSpan.FromHours(2);
    public TimeSpan Labels { get; init; } = TimeSpan.FromHours(1);
}