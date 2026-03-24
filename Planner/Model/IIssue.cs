using System.Collections.Immutable;

namespace Planner.Model;

public interface IIssueCore
{
    string Id { get; init; }
    string Key { get; set; }
    string Self { get; set; }
    string ProjectKey { get; set; }
    string Summary { get; set; }
    StatusModel Status { get; set; }
}

public interface IHasAssignee : IIssueCore { UserModel? Assignee { get; set; } }
public interface IHasType : IIssueCore { TypeModel Type { get; set; } }
public interface IHasComponents : IIssueCore { ImmutableArray<ComponentModel> Components { get; set; } }
public interface IHasLabels : IIssueCore { ImmutableArray<LabelModel> Labels { get; set; } }
public interface IHasFixVersions : IIssueCore { ImmutableArray<FixVersionModel> FixVersions { get; set; } }
public interface IHasTransitions : IIssueCore { ImmutableArray<TransitionModel> Transitions { get; set; } }
public interface IHasDates : IIssueCore
{
    DateOnly? StartDate { get; set; }
    DateOnly? EndDate { get; set; }
    DateOnly? DueDate { get; set; }
}
public interface IHasStats : IIssueCore { TimeStats Stats { get; set; } }

