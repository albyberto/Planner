using Microsoft.Extensions.Logging;
using Planner.Clients;

namespace Planner.Clients.Core;

public class IssueWriteService(JiraWriteClient writeClient, ILogger<IssueWriteService> logger)
{
    public async Task UpdateIssueAsync(string issueKey, object fields, CancellationToken ct = default)
    {
        try
        {
            await writeClient.UpdateIssueAsync(issueKey, fields, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating issue {IssueKey}", issueKey);
            throw;
        }
    }

    public async Task TransitionAsync(string issueKey, string transitionId, CancellationToken ct = default)
    {
        try
        {
            await writeClient.TransitionAsync(issueKey, transitionId, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error transitioning issue {IssueKey} to {TransitionId}", issueKey, transitionId);
            throw;
        }
    }

    public async Task UpdateVersionAsync(string versionId, string name, string? description = null, DateOnly? startDate = null, DateOnly? releaseDate = null, CancellationToken ct = default)
    {
        try
        {
            await writeClient.UpdateVersionAsync(versionId, name, description, startDate, releaseDate, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating version {VersionId}", versionId);
            throw;
        }
    }
}
