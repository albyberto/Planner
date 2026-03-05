using System.Collections.Immutable;
using Planner.Domain;

namespace Planner.Model;

public record IssueModel
{
    public string Id { get; init; }
    public string Key { get; init; }
    public string Summary { get; init; }
    public StatusModel Status { get; init; }
    public UserModel Assignee { get; init; }
    public IssueTypeModel Type { get; init; }
    public ImmutableList<ComponentModel> Components { get; init; }
    public ImmutableList<LabelModel> Labels { get; init; }
    public ImmutableList<FixVersionModel> FixVersions { get; init; }
    public DateOnly? StartDate { get; init; }

    public TimeStats Stats { get; init; }

    private readonly Lazy<Task<IReadOnlyList<Transition>>> _transitionsLoader;

    public IssueModel(Issue dto, Func<Task<IReadOnlyList<Transition>>> loadTransitions)
    {
        Id = dto.Id;
        Key = dto.Key;
        Summary = dto.Fields.Summary;
        Status = new(dto.Fields.Status);
        Assignee = new(dto.Fields.Assignee);
        Type = new(dto.Fields.IssueType);
        Components = dto.Fields.Components.Select(component => new ComponentModel(component)).ToImmutableList();
        Labels = dto.Fields.Labels.Select(label => new LabelModel(label)).ToImmutableList();
        FixVersions = dto.Fields.FixVersions.Select(version => new FixVersionModel(version)).ToImmutableList() ?? [];
        StartDate = dto.Fields.StartDate;

        var originalEstimate = dto.Fields.TimeTracking.OriginalEstimateSeconds ?? 0;
        var timeSpent = dto.Fields.Worklog.Worklogs.Sum(w => w.TimeSpentSeconds ?? 0);
        
        Stats = new(originalEstimate, timeSpent);

        _transitionsLoader = new(loadTransitions);
    }

    public async Task<IReadOnlyList<TransitionModel>> GetTransitionsAsync()
    {
        var transitions = await _transitionsLoader.Value;
        
        return transitions
            .Select(transition => new TransitionModel(transition))
            .ToImmutableList();
    }
}