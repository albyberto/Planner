using Planner.Clients.Domain;
using Planner.Model;

namespace Planner.Mappers;

public static class DashboardIssueMapper
{
    public static DashboardIssueModel ToDashboardModel(this Issue issue)
    {
        var originalEstimate = issue.Fields?.TimeTracking?.OriginalEstimateSeconds ?? 0;
        var explicitRemaining = issue.Fields?.TimeTracking?.RemainingEstimateSeconds;
        var totalTimeSpent = issue.Fields?.Worklog?.Worklogs.Sum(w => w.TimeSpentSeconds ?? 0) ?? 0;
        
        var assigneeId = issue.Fields?.Assignee?.AccountId;
        var assigneeTimeSpent = assigneeId != null 
            ? issue.Fields?.Worklog?.Worklogs
                .Where(w => w.Author?.AccountId == assigneeId)
                .Sum(w => w.TimeSpentSeconds ?? 0) ?? 0
            : 0;

        var stats = new TimeStats(originalEstimate, totalTimeSpent, assigneeTimeSpent, explicitRemaining);

        return new(
            Id: issue.Id,
            Key: issue.Key,
            Self: issue.Self ?? string.Empty,
            ProjectKey: issue.Fields?.Project?.Key ?? string.Empty,
            Summary: issue.Fields?.Summary ?? string.Empty,
            Status: new(issue.Fields?.Status ?? throw new InvalidOperationException("Status is required for a dashboard issue.")),
            Assignee: issue.Fields?.Assignee != null ? new UserModel(issue.Fields.Assignee) : null,
            Type: new(issue.Fields?.Type ?? throw new InvalidOperationException("Type is required for a dashboard issue.")),
            Components: issue.Fields?.Components != null ? [..issue.Fields.Components.Select(c => new ComponentModel(c))] : [],
            Labels: issue.Fields?.Labels != null ? [..issue.Fields.Labels.Select(l => new LabelModel(l))] : [],
            FixVersions: issue.Fields?.FixVersions != null ? [..issue.Fields.FixVersions.Select(v => new FixVersionModel(v))] : [],
            Transitions: issue.Transitions != null ? [..issue.Transitions.Select(t => new TransitionModel(t))] : [],
            StartDate: issue.Fields?.StartDate,
            EndDate: issue.Fields?.EndDate,
            DueDate: issue.Fields?.DueDate,
            Stats: stats
        );
    }
}