using Planner.Builders;
using Planner.Clients.Core;

namespace Planner.Facades;

public class IssueFacade(IssueWriteService writeService)
{
    public Task UpdateSummaryAsync(string issueKey, string summary, CancellationToken ct = default)
    {
        var update = new IssueUpdateBuilder().SetSummary(summary);
        return writeService.UpdateIssueAsync(issueKey, update.Fields, ct);
    }

    public Task<string> CreateEpicAsync(string projectKey, string summary, string epicName, CancellationToken ct = default)
    {
        return writeService.CreateEpicAsync(projectKey, summary, epicName, ct);
    }

    public Task UpdateEpicAsync(string issueKey, string epicKey, CancellationToken ct = default)
    {
        // Try parent first
        var update = new IssueUpdateBuilder().SetCustomField("parent", new { key = epicKey });
        return writeService.UpdateIssueAsync(issueKey, update.Fields, ct);
    }

    public Task UpdateTypeAsync(string issueKey, string issueTypeId, CancellationToken ct = default)
    {
        var update = new IssueUpdateBuilder().SetIssueType(issueTypeId);
        return writeService.UpdateIssueAsync(issueKey, update.Fields, ct);
    }

    public Task TransitionAsync(string issueKey, string transitionId, CancellationToken ct = default)
    {
        return writeService.TransitionAsync(issueKey, transitionId, ct);
    }

    public Task UpdateVersionAsync(string versionId, string name, string? description = null, DateOnly? startDate = null, DateOnly? releaseDate = null, CancellationToken ct = default)
    {
        return writeService.UpdateVersionAsync(versionId, name, description, startDate, releaseDate, ct);
    }

    public Task UpdateStartDateAsync(string issueKey, DateOnly? startDate, CancellationToken ct = default)
    {
        var update = new IssueUpdateBuilder().SetCustomField("customfield_10117", startDate?.ToString("yyyy-MM-dd"));
        return writeService.UpdateIssueAsync(issueKey, update.Fields, ct);
    }

    public Task UpdateEndDateAsync(string issueKey, DateOnly? endDate, CancellationToken ct = default)
    {
        var update = new IssueUpdateBuilder().SetCustomField("customfield_10118", endDate?.ToString("yyyy-MM-dd"));
        return writeService.UpdateIssueAsync(issueKey, update.Fields, ct);
    }

    public Task UpdateOriginalEstimationAsync(string issueKey, string estimate, CancellationToken ct = default)
    {
        var update = new IssueUpdateBuilder().SetTimeTracking(originalEstimate: estimate);
        return writeService.UpdateIssueAsync(issueKey, update.Fields, ct);
    }

    public Task UpdateRemainingEstimationAsync(string issueKey, string remaining, CancellationToken ct = default)
    {
        var update = new IssueUpdateBuilder().SetTimeTracking(remainingEstimate: remaining);
        return writeService.UpdateIssueAsync(issueKey, update.Fields, ct);
    }

    public Task UpdateLabelsAsync(string issueKey, IEnumerable<string> labels, CancellationToken ct = default)
    {
        var update = new IssueUpdateBuilder().SetLabels(labels);
        return writeService.UpdateIssueAsync(issueKey, update.Fields, ct);
    }

    public Task UpdateComponentsAsync(string issueKey, IEnumerable<string> componentIds, CancellationToken ct = default)
    {
        var update = new IssueUpdateBuilder().SetComponents(componentIds);
        return writeService.UpdateIssueAsync(issueKey, update.Fields, ct);
    }
}
