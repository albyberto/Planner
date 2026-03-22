// using System.Collections.Immutable;
// using Planner.Clients.Domain;
//
// namespace Planner.Model;
//
// public record IssueModel
// {
//     public string Id { get; set; }
//     public string Key { get; set; }
//     public string Self { get; set; }
//     public string ProjectKey { get; set; }
//     public string Summary { get; set; }
//     public StatusModel Status { get; set; }
//     public UserModel Assignee { get; set; }
//     public TypeModel Type { get; set; }
//     public ImmutableArray<ComponentModel> Components { get; set; }
//     public ImmutableArray<LabelModel> Labels { get; set; }
//     public ImmutableArray<FixVersionModel> FixVersions { get; set; }
//     
//     public ImmutableArray<TransitionModel> Transitions { get; set; }
//
//     public DateOnly? StartDate { get; set; }
//     public DateOnly? EndDate { get; set; }
//     public DateOnly? DueDate { get; set; }
//
//     public TimeStats Stats { get; set; }
//
//     public IssueModel(Issue dto)
//     {
//         Id = dto.Id;
//         Key = dto.Key;
//         Self = dto.Self;
//         ProjectKey = dto.Fields.Project?.Key ?? string.Empty; 
//         Summary = dto.Fields.Summary ?? string.Empty;
//         Status = new(dto.Fields.Status);
//         Assignee = new(dto.Fields.Assignee);
//         Type = new(dto.Fields.Type);
//         
//         Components = dto.Fields.Components != null ? [..dto.Fields.Components.Select(component => new ComponentModel(component))] : [];
//         Labels = dto.Fields.Labels != null ? [..dto.Fields.Labels.Select(label => new LabelModel(label))] : [];
//         FixVersions = dto.Fields.FixVersions != null ? [..dto.Fields.FixVersions.Select(version => new FixVersionModel(version))] : [];
//         
//         Transitions = dto.Transitions != null 
//             ? [..dto.Transitions.Select(transition => new TransitionModel(transition))] 
//             : [];
//
//         StartDate = dto.Fields.StartDate;
//         EndDate = dto.Fields.EndDate;
//         DueDate = dto.Fields.DueDate;
//
//         var originalEstimate = dto.Fields.TimeTracking?.OriginalEstimateSeconds ?? 0;
//         var timeSpent = dto.Fields?.Worklog?.Worklogs?.Sum(w => w.TimeSpentSeconds ?? 0) ?? 0;
//         var remainingEstimate = dto.Fields.TimeTracking?.RemainingEstimateSeconds;
//         
//         Stats = new(originalEstimate, timeSpent, remainingEstimate);
//     }
// }