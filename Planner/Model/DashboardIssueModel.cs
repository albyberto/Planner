using System;
using System.Collections.Immutable;

namespace Planner.Model;

public class DashboardIssueModel : IIssue
{
    public string Id { get; set; }
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
        StartDate = startDate;
        EndDate = endDate;
        DueDate = dueDate;
        Stats = stats;
    }
}