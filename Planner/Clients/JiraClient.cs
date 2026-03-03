using System.Collections.Frozen;
using Microsoft.Extensions.Options;
using Planner.Models;
using Planner.Models.Requests;

namespace Planner.Clients;

public class JiraClient(HttpClient httpClient, IOptions<JiraQueryOptions> options, ILogger<JiraClient> logger)
{
    private readonly JiraQueryOptions _settings = options.Value;

    public async Task<FrozenSet<Status>> GetProjectStatusesAsync(CancellationToken cancellationToken = default)
    {
        var tasks = _settings.ProjectKeys.Select(async project =>
        {
            try
            {
                var issueTypes = await httpClient.GetFromJsonAsync<List<StatusRequest>>($"project/{project}/statuses", cancellationToken);
                return issueTypes ?? [];   
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error in GetProjectStatusesAsync");
                throw;
            }
        });

        var results = await Task.WhenAll(tasks);

        return results
            .SelectMany(requests => requests)
            .SelectMany(request => request.Statuses)
            .DistinctBy(status => status.Id)
            .OrderBy(name => name)
            .ToFrozenSet();
    }

    public async Task<FrozenSet<AssigneeRequest>> GetProjectAssigneesAsync(CancellationToken cancellationToken = default)
    {
        var tasks = _settings.ProjectKeys.Select(async projectKey =>
        {
            try
            {
                var assignees = await httpClient.GetFromJsonAsync<List<AssigneeRequest>>($"user/assignable/search?project={projectKey}", cancellationToken);
                return assignees ?? [];
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error in GetProjectAssigneesAsync");
                throw;
            }
        });

        var results = await Task.WhenAll(tasks);

        return results
            .SelectMany(users => users)
            .Where(user => !string.IsNullOrEmpty(user.AccountId))
            .DistinctBy(user => user.AccountId)
            .OrderBy(user => user.DisplayName)
            .ToFrozenSet();
    }
}