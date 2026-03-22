using System.Collections.Immutable;

namespace Planner.Model;

public record DashboardIssueModel(
    string Id,
    string Key,
    string Self,
    string ProjectKey,
    string Summary,
    StatusModel Status,
    UserModel? Assignee,
    TypeModel Type,
    ImmutableArray<ComponentModel> Components,
    ImmutableArray<LabelModel> Labels,
    ImmutableArray<FixVersionModel> FixVersions,
    ImmutableArray<TransitionModel> Transitions,
    DateOnly? StartDate,
    DateOnly? EndDate,
    DateOnly? DueDate,
    TimeStats Stats
);