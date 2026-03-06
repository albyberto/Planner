using Planner.Builders;

namespace Planner.Clients;

public class JiraWriteClient(HttpClient httpClient, ILogger<JiraWriteClient> logger)
{
    public async Task UpdateIssueAsync(string issueKey, IssueUpdateBuilder update, CancellationToken ct = default)
    {
        if (!update.HasChanges) return;

        var response = await httpClient.PutAsJsonAsync($"issue/{issueKey}", new { fields = update.Fields }, ct);
        await EnsureSuccessAsync(response, $"UpdateIssueAsync({issueKey})", ct);
    }

    public async Task TransitionAsync(string issueKey, string transitionId, CancellationToken ct = default)
    {
        var body = new { transition = new { id = transitionId } };
        var response = await httpClient.PostAsJsonAsync($"issue/{issueKey}/transitions", body, ct);
        await EnsureSuccessAsync(response, $"TransitionAsync({issueKey}, {transitionId})", ct);
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response, string operation, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode) return;

        var errorContent = await response.Content.ReadAsStringAsync(ct);
        logger.LogError("Jira API error in {Operation}: {StatusCode} - {Error}", operation, response.StatusCode, errorContent);
        response.EnsureSuccessStatusCode();
    }
}