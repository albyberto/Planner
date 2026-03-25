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

        var epic = issue.Fields?.Parent != null
            ? new EpicModel(issue.Fields.Parent.Key, issue.Fields.Parent.Fields?.Summary ?? string.Empty)
            : (!string.IsNullOrWhiteSpace(issue.Fields?.EpicLink) ? new EpicModel(issue.Fields.EpicLink, issue.Fields.EpicLink) : null);

        return new(
             issue.Id,
             issue.Key,
             issue.Self ?? string.Empty,
             issue.Fields?.Project?.Key ?? string.Empty,
             issue.Fields?.Summary ?? string.Empty,
             new(issue.Fields?.Status ?? throw new InvalidOperationException("Status is required for a dashboard issue.")),
             issue.Fields?.Assignee != null ? new UserModel(issue.Fields.Assignee) : null,
             new(issue.Fields?.Type ?? throw new InvalidOperationException("Type is required for a dashboard issue.")),
             issue.Fields?.Components != null ? [..issue.Fields.Components.Select(c => new ComponentModel(c))] : [],
             issue.Fields?.Labels != null ? [..issue.Fields.Labels.Select(l => new LabelModel(l))] : [],
             issue.Fields?.FixVersions != null ? [..issue.Fields.FixVersions.Select(v => new FixVersionModel(v))] : [],
             issue.Transitions != null ? [..issue.Transitions.Select(t => new TransitionModel(t))] : [],
             epic,
             issue.Fields?.StartDate,
             issue.Fields?.EndDate,
             issue.Fields?.DueDate,
             stats
        );
    }
}