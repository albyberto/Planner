namespace Planner.Builders;

public class IssueUpdateBuilder
{
    public Dictionary<string, object> Fields { get; } = new();

    public IssueUpdateBuilder SetSummary(string summary)
    {
        Fields["summary"] = summary;
        return this;
    }

    public IssueUpdateBuilder SetIssueType(string issueTypeId)
    {
        Fields["issuetype"] = new { id = issueTypeId };
        return this;
    }

    public IssueUpdateBuilder SetComponents(IEnumerable<string> componentIds)
    {
        Fields["components"] = componentIds.Select(id => new { id }).ToArray();
        return this;
    }

    public IssueUpdateBuilder SetLabels(IEnumerable<string> labels)
    {
        Fields["labels"] = labels.ToArray();
        return this;
    }

    public IssueUpdateBuilder SetTimeTracking(string? originalEstimate = null, string? remainingEstimate = null)
    {
        var timeTracking = new Dictionary<string, string>();
        if (originalEstimate is not null) timeTracking["originalEstimate"] = originalEstimate;
        if (remainingEstimate is not null) timeTracking["remainingEstimate"] = remainingEstimate;
        
        if (timeTracking.Count > 0)
        {
            Fields["timetracking"] = timeTracking;
        }
        return this;
    }

    public IssueUpdateBuilder SetCustomField(string fieldId, object? value)
    {
        Fields[fieldId] = value;
        return this;
    }
    
    public bool HasChanges => Fields.Count > 0;
}