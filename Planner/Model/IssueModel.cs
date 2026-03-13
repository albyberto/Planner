using System.Collections.Immutable;
using Planner.Domain;

namespace Planner.Model;

public record IssueModel
{
    public string Id { get; set; }
    public string Key { get; set; }
    public string Self { get; set; }
    public string ProjectKey { get; set; }
    public string Summary { get; set; }
    public StatusModel Status { get; set; }
    public UserModel Assignee { get; set; }
    public TypeModel Type { get; set; }
    public ImmutableArray<ComponentModel> Components { get; set; }
    public ImmutableArray<LabelModel> Labels { get; set; }
    public ImmutableArray<FixVersionModel> FixVersions { get; set; }
    // public ImmutableArray<IssueCommentModel> Comments { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateOnly? DueDate { get; set; }

    public TimeStats Stats { get; set; }

    private readonly Lazy<Task<IReadOnlyList<Transition>>> _transitionsLoader;

    public IssueModel(Issue dto, Func<Task<IReadOnlyList<Transition>>> loadTransitions)
    {
        Id = dto.Id;
        Key = dto.Key;
        Self = dto.Self;
        ProjectKey = dto.Fields.Project.Key;
        Summary = dto.Fields.Summary;
        Status = new(dto.Fields.Status);
        Assignee = new(dto.Fields.Assignee);
        Type = new(dto.Fields.Type);
        Components = [..dto.Fields.Components.Select(component => new ComponentModel(component))];
        Labels = [..dto.Fields.Labels.Select(label => new LabelModel(label))];
        FixVersions = [..dto.Fields.FixVersions.Select(version => new FixVersionModel(version))];
        // Comments = dto.Fields.Comment?.Body.ContentList.Select(comment => new IssueCommentModel(comment)).ToImmutableArray() ?? [];
        StartDate = dto.Fields.StartDate;
        EndDate = dto.Fields.EndDate;
        DueDate = dto.Fields.DueDate;

        var originalEstimate = dto.Fields.TimeTracking?.OriginalEstimateSeconds ?? 0;
        // var timeSpent = dto.Fields?.Worklog?.Worklogs?.Sum(w => w.TimeSpentSeconds ?? 0) ?? 0;
        // var remainingEstimate = dto.Fields.TimeTracking?.RemainingEstimateSeconds;
        
        // Stats = new(originalEstimate, timeSpent, remainingEstimate);

        _transitionsLoader = new(loadTransitions);
    }

    public async Task<IReadOnlyList<TransitionModel>> GetTransitionsAsync()
    {
        var transitions = await _transitionsLoader.Value;
        
        return transitions
            .Select(transition => new TransitionModel(transition))
            .ToImmutableArray();
    }
}