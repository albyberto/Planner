using System.Collections.Immutable;

namespace Planner.Model;

public interface IIssue
{
    string Id { get; set; }
    string Key { get; set; }
    string Self { get; set; }
    string ProjectKey { get; set; }
    string Summary { get; set; }
    StatusModel Status { get; set; }
    UserModel? Assignee { get; set; }
    TypeModel Type { get; set; }
    ImmutableArray<ComponentModel> Components { get; set; }
    ImmutableArray<LabelModel> Labels { get; set; }
    ImmutableArray<FixVersionModel> FixVersions { get; set; }
    ImmutableArray<TransitionModel> Transitions { get; set; }
    DateOnly? StartDate { get; set; }
    DateOnly? EndDate { get; set; }
    DateOnly? DueDate { get; set; }
    TimeStats Stats { get; set; }
}
