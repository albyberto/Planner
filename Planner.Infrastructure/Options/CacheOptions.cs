namespace Planner.Infrastructure.Options;

public class CacheOptions
{
    public const string SectionName = nameof(CacheOptions);

    public TimeSpan Projects { get; init; } = TimeSpan.FromMinutes(30);
    public TimeSpan Types { get; init; } = TimeSpan.FromMinutes(30);
    public TimeSpan Assignees { get; init; } = TimeSpan.FromMinutes(30);
    public TimeSpan Components { get; init; } = TimeSpan.FromMinutes(30);
    public TimeSpan Labels { get; init; } = TimeSpan.FromMinutes(60);
}