using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace Planner.Clients;

public class JiraWriteClient(HttpClient httpClient, ILogger<JiraWriteClient> logger)
{
    public async Task UpdateIssueAsync(string issueKey, object fields, CancellationToken ct = default)
    {
        var response = await httpClient.PutAsJsonAsync($"issue/{issueKey}", new { fields = fields }, ct);
        await EnsureSuccessAsync(response, $"UpdateIssueAsync({issueKey})", ct);
    }

    public async Task<string> CreateEpicAsync(string projectKey, string summary, string name, CancellationToken ct = default)
    {
        var body = new
        {
            fields = new Dictionary<string, object>
            {
                ["project"] = new { key = projectKey },
                ["issuetype"] = new { name = "Epic" },
                ["summary"] = summary,
                ["customfield_10011"] = name // Epic Name (fallback for older instances, often ignored safely if not needed)
            }
        };
        var response = await httpClient.PostAsJsonAsync("issue", body, ct);
        await EnsureSuccessAsync(response, $"CreateEpicAsync({projectKey})", ct);
        
        var result = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>(cancellationToken: ct);
        return result.GetProperty("key").GetString()!;
    }

    public async Task TransitionAsync(string issueKey, string transitionId, CancellationToken ct = default)
    {
        var body = new { transition = new { id = transitionId } };
        var response = await httpClient.PostAsJsonAsync($"issue/{issueKey}/transitions", body, ct);
        await EnsureSuccessAsync(response, $"TransitionAsync({issueKey}, {transitionId})", ct);
    }

    public async Task UpdateVersionAsync(string versionId, string name, string? description = null, DateOnly? startDate = null, DateOnly? releaseDate = null, CancellationToken ct = default)
    {
        var body = new 
        { 
            name, 
            description,
            startDate = startDate?.ToString("yyyy-MM-dd"),
            releaseDate = releaseDate?.ToString("yyyy-MM-dd")
        };
        var response = await httpClient.PutAsJsonAsync($"version/{versionId}", body, ct);
        await EnsureSuccessAsync(response, $"UpdateVersionAsync({versionId})", ct);
    }

    // public async Task<Planner.Clients.Domain.IssueComment?> AddCommentAsync(string issueKey, object adfBody, CancellationToken ct = default)
    // {
    //     var body = new { body = adfBody };
    //     var response = await httpClient.PostAsJsonAsync($"issue/{issueKey}/comment", body, ct);
    //     await EnsureSuccessAsync(response, $"AddCommentAsync({issueKey})", ct);
    //     
    //     return await response.Content.ReadFromJsonAsync<Planner.Clients.Domain.IssueComment>(cancellationToken: ct);
    // }

    public async Task UpdateCommentAsync(string issueKey, string commentId, object adfBody, CancellationToken ct = default)
    {
        var body = new { body = adfBody };
        var response = await httpClient.PutAsJsonAsync($"issue/{issueKey}/comment/{commentId}", body, ct);
        await EnsureSuccessAsync(response, $"UpdateCommentAsync({issueKey}, {commentId})", ct);
    }

    public async Task DeleteCommentAsync(string issueKey, string commentId, CancellationToken ct = default)
    {
        var response = await httpClient.DeleteAsync($"issue/{issueKey}/comment/{commentId}", ct);
        await EnsureSuccessAsync(response, $"DeleteCommentAsync({issueKey}, {commentId})", ct);
    }
    
    private async Task EnsureSuccessAsync(HttpResponseMessage response, string operation, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode) return;

        var errorContent = await response.Content.ReadAsStringAsync(ct);
        logger.LogError("Jira API error in {Operation}: {StatusCode} - {Error}", operation, response.StatusCode, errorContent);
        response.EnsureSuccessStatusCode();
    }
}