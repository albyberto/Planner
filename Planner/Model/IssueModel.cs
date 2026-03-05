using System.Collections.Immutable;
using Planner.Domain;

namespace Planner.Model;

public record IssueModel
{
    public string Id { get; init; }
    public string Key { get; init; }
    public string Summary { get; init; }
    public Status Status { get; init; }
    public User Assignee { get; init; }
    public IssueType IssueType { get; init; }
    public ImmutableList<Component> Components { get; init; }
    public ImmutableList<string> Labels { get; init; }
    public ImmutableList<FixVersion> FixVersions { get; init; }
    public DateOnly? StartDate { get; init; }

    public TimeStats Stats { get; init; }

    private readonly Lazy<Task<IReadOnlyList<Transition>>> _transitionsLoader;

    public IssueModel(Issue dto, Func<Task<IReadOnlyList<Transition>>> loadTransitions)
    {
        Id = dto.Id;
        Key = dto.Key;
        Summary = dto.Fields.Summary;
        Status = dto.Fields.Status;
        Assignee = dto.Fields.Assignee;
        IssueType = dto.Fields.IssueType;
        Components = dto.Fields.Components?.ToImmutableList() ?? [];
        Labels = dto.Fields.Labels?.ToImmutableList() ?? [];
        FixVersions = dto.Fields.FixVersions?.ToImmutableList() ?? [];
        StartDate = dto.Fields.StartDate;

        var originalEstimate = dto.Fields.TimeTracking?.OriginalEstimateSeconds ?? 0;
        var timeSpent = dto.Fields.Worklog?.Worklogs?.Sum(w => w.TimeSpentSeconds ?? 0) ?? 0;
        
        Stats = new(originalEstimate, timeSpent);

        _transitionsLoader = new(loadTransitions);
    }

    public Task<IReadOnlyList<Transition>> GetTransitionsAsync() => _transitionsLoader.Value;
}