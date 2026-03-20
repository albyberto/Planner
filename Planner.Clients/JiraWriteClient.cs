// using System.Net.Http.Json;
// using Microsoft.Extensions.Logging;
//
// namespace Planner.Infrastructure.Clients;
//
// public class JiraWriteClient(HttpClient httpClient, ILogger<JiraWriteClient> logger)
// {
//     public async Task UpdateIssueAsync(string issueKey, IssueUpdateBuilder update, CancellationToken ct = default)
//     {
//         if (!update.HasChanges) return;
//
//         var response = await httpClient.PutAsJsonAsync($"issue/{issueKey}", new { fields = update.Fields }, ct);
//         await EnsureSuccessAsync(response, $"UpdateIssueAsync({issueKey})", ct);
//     }
//
//     public async Task TransitionAsync(string issueKey, string transitionId, CancellationToken ct = default)
//     {
//         var body = new { transition = new { id = transitionId } };
//         var response = await httpClient.PostAsJsonAsync($"issue/{issueKey}/transitions", body, ct);
//         await EnsureSuccessAsync(response, $"TransitionAsync({issueKey}, {transitionId})", ct);
//     }
//
//     public async Task UpdateVersionAsync(string versionId, string name, string? description = null, DateOnly? startDate = null, DateOnly? releaseDate = null, CancellationToken ct = default)
//     {
//         var body = new 
//         { 
//             name, 
//             description,
//             startDate = startDate?.ToString("yyyy-MM-dd"),
//             releaseDate = releaseDate?.ToString("yyyy-MM-dd")
//         };
//         var response = await httpClient.PutAsJsonAsync($"version/{versionId}", body, ct);
//         await EnsureSuccessAsync(response, $"UpdateVersionAsync({versionId})", ct);
//     }
//
//     public async Task<Planner.Model.IssueCommentModel?> AddCommentAsync(string issueKey, object adfBody, CancellationToken ct = default)
//     {
//         var body = new { body = adfBody };
//         var response = await httpClient.PostAsJsonAsync($"issue/{issueKey}/comment", body, ct);
//         await EnsureSuccessAsync(response, $"AddCommentAsync({issueKey})", ct);
//         
//         var dto = await response.Content.ReadFromJsonAsync<Planner.Domain.IssueComment>(cancellationToken: ct);
//         return dto != null ? new Planner.Model.IssueCommentModel(dto) : null;
//     }
//
//     public async Task UpdateCommentAsync(string issueKey, string commentId, object adfBody, CancellationToken ct = default)
//     {
//         var body = new { body = adfBody };
//         var response = await httpClient.PutAsJsonAsync($"issue/{issueKey}/comment/{commentId}", body, ct);
//         await EnsureSuccessAsync(response, $"UpdateCommentAsync({issueKey}, {commentId})", ct);
//     }
//
//     public async Task DeleteCommentAsync(string issueKey, string commentId, CancellationToken ct = default)
//     {
//         var response = await httpClient.DeleteAsync($"issue/{issueKey}/comment/{commentId}", ct);
//         await EnsureSuccessAsync(response, $"DeleteCommentAsync({issueKey}, {commentId})", ct);
//     }
//     
//     private async Task EnsureSuccessAsync(HttpResponseMessage response, string operation, CancellationToken ct)
//     {
//         if (response.IsSuccessStatusCode) return;
//
//         var errorContent = await response.Content.ReadAsStringAsync(ct);
//         logger.LogError("Jira API error in {Operation}: {StatusCode} - {Error}", operation, response.StatusCode, errorContent);
//         response.EnsureSuccessStatusCode();
//     }
// }