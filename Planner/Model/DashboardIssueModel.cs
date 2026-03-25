using System;
using System.Collections.Immutable;

namespace Planner.Model;

public record EpicModel(string Key, string Summary) : IComparable<EpicModel>
{
    public int CompareTo(EpicModel? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return string.Compare(Summary, other.Summary, StringComparison.OrdinalIgnoreCase);
    }
    
    public override string ToString() => Summary;
}

public record DashboardIssueModel : IIssueCore, IHasAssignee, IHasType, IHasComponents, IHasLabels, IHasFixVersions, IHasTransitions, IHasEpic, IHasDates, IHasStats
{
    public string Id { get; init; }
    public string Key { get; set; }
    public string Self { get; set; }
    public string ProjectKey { get; set; }
    public string Summary { get; set; }
    public StatusModel Status { get; set; }
    public UserModel? Assignee { get; set; }
    public TypeModel Type { get; set; }
    public ImmutableArray<ComponentModel> Components { get; set; }
    public ImmutableArray<LabelModel> Labels { get; set; }
    public ImmutableArray<FixVersionModel> FixVersions { get; set; }
    public ImmutableArray<TransitionModel> Transitions { get; set; }
    public EpicModel? Epic { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public TimeStats Stats { get; set; }

    public DashboardIssueModel(
        string id,
        string key,
        string self,
        string projectKey,
        string summary,
        StatusModel status,
        UserModel? assignee,
        TypeModel type,
        ImmutableArray<ComponentModel> components,
        ImmutableArray<LabelModel> labels,
        ImmutableArray<FixVersionModel> fixVersions,
        ImmutableArray<TransitionModel> transitions,
        EpicModel? epic,
        DateOnly? startDate,
        DateOnly? endDate,
        DateOnly? dueDate,
        TimeStats stats)
    {
        Id = id;
        Key = key;
        Self = self;
        ProjectKey = projectKey;
        Summary = summary;
        Status = status;
        Assignee = assignee;
        Type = type;
        Components = components;
        Labels = labels;
        FixVersions = fixVersions;
        Transitions = transitions;
        Epic = epic;
        StartDate = startDate;
        EndDate = endDate;
        DueDate = dueDate;
        Stats = stats;
    }

    public virtual bool Equals(DashboardIssueModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Id == other.Id
            && Key == other.Key
            && Self == other.Self
            && ProjectKey == other.ProjectKey
            && Summary == other.Summary
            && Status == other.Status
            && Assignee == other.Assignee
            && Type == other.Type
            && Epic == other.Epic
            && StartDate == other.StartDate
            && EndDate == other.EndDate
            && DueDate == other.DueDate
            && Stats == other.Stats
            && Components.SequenceEqual(other.Components)
            && Labels.SequenceEqual(other.Labels)
            && FixVersions.SequenceEqual(other.FixVersions)
            && Transitions.SequenceEqual(other.Transitions);
    }

    public override int GetHashCode() => Id.GetHashCode();
}