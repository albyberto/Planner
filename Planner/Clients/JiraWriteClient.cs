namespace Planner.Clients;

public class JiraWriteClient(HttpClient httpClient, ILogger<JiraWriteClient> logger)
{
    // ───── Field updates ─────

    /// <summary>Updates summary, original estimate, remaining estimate on an issue.</summary>
    public async Task UpdateFieldsAsync(string issueKey, string? summary = null, string? originalEstimate = null, string? remainingEstimate = null, CancellationToken ct = default)
    {
        var fields = new Dictionary<string, object>();

        if (summary is not null)
            fields["summary"] = summary;

        if (originalEstimate is not null || remainingEstimate is not null)
        {
            var timeTracking = new Dictionary<string, string>();
            if (originalEstimate is not null) timeTracking["originalEstimate"] = originalEstimate;
            if (remainingEstimate is not null) timeTracking["remainingEstimate"] = remainingEstimate;
            fields["timetracking"] = timeTracking;
        }

        if (fields.Count == 0) return;

        await PutFieldsAsync(issueKey, fields, ct);
    }

    /// <summary>Updates the issue type.</summary>
    public async Task UpdateIssueTypeAsync(string issueKey, string issueTypeId, CancellationToken ct = default)
    {
        await PutFieldsAsync(issueKey, new Dictionary<string, object>
        {
            ["issuetype"] = new { id = issueTypeId }
        }, ct);
    }

    /// <summary>Replaces the components list on the issue.</summary>
    public async Task UpdateComponentsAsync(string issueKey, IEnumerable<string> componentIds, CancellationToken ct = default)
    {
        await PutFieldsAsync(issueKey, new Dictionary<string, object>
        {
            ["components"] = componentIds.Select(id => new { id }).ToArray()
        }, ct);
    }

    /// <summary>Replaces the labels list on the issue.</summary>
    public async Task UpdateLabelsAsync(string issueKey, IEnumerable<string> labels, CancellationToken ct = default)
    {
        await PutFieldsAsync(issueKey, new Dictionary<string, object>
        {
            ["labels"] = labels.ToArray()
        }, ct);
    }

    // ───── Time logging ─────

    /// <summary>Adds a worklog entry (time spent) to an issue.</summary>
    public async Task AddWorklogAsync(string issueKey, string timeSpent, string? comment = null, CancellationToken ct = default)
    {
        var body = new Dictionary<string, object> { ["timeSpent"] = timeSpent };

        if (!string.IsNullOrWhiteSpace(comment))
        {
            body["comment"] = new
            {
                type = "doc", version = 1,
                content = new[] { new { type = "paragraph", content = new[] { new { type = "text", text = comment } } } }
            };
        }

        var response = await httpClient.PostAsJsonAsync($"issue/{issueKey}/worklog", body, ct);
        await EnsureSuccessAsync(response, $"AddWorklogAsync({issueKey})", ct);
    }

    // ───── Status transition ─────

    /// <summary>Transitions an issue to a new status.</summary>
    public async Task TransitionAsync(string issueKey, string transitionId, CancellationToken ct = default)
    {
        var body = new { transition = new { id = transitionId } };
        var response = await httpClient.PostAsJsonAsync($"issue/{issueKey}/transitions", body, ct);
        await EnsureSuccessAsync(response, $"TransitionAsync({issueKey}, {transitionId})", ct);
    }

    // ───── Helpers ─────

    private async Task PutFieldsAsync(string issueKey, Dictionary<string, object> fields, CancellationToken ct)
    {
        var response = await httpClient.PutAsJsonAsync($"issue/{issueKey}", new { fields }, ct);
        await EnsureSuccessAsync(response, $"PutFieldsAsync({issueKey})", ct);
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response, string operation, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode) return;

        var errorContent = await response.Content.ReadAsStringAsync(ct);
        logger.LogError("Jira API error in {Operation}: {StatusCode} - {Error}", operation, response.StatusCode, errorContent);
        response.EnsureSuccessStatusCode();
    }
}

