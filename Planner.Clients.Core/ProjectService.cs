using System.Collections.Immutable;
using Microsoft.Extensions.Options;
using Planner.Clients.Core.Options;
using Planner.Clients.Domain;
using ZiggyCreatures.Caching.Fusion;
using Component = Planner.Clients.Domain.Component;
using Type = Planner.Clients.Domain.Type;

namespace Planner.Clients.Core;

public class ProjectService(JiraProjectClient jiraClient, IFusionCache cache, IOptions<CacheOptions> cacheOptions)
{
    private readonly CacheOptions _cacheOptions = cacheOptions.Value;

    public async Task<ImmutableArray<Project>> GetProjectsAsync(CancellationToken cancellationToken = default) =>
        await cache.GetOrSetAsync("jira:projects:all", async ct => await jiraClient.GetProjectsAsync(ct), new FusionCacheEntryOptions { Duration = _cacheOptions.Projects }, cancellationToken);

    public async Task<ImmutableArray<Type>> GetTypesAndStatusesAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var statusesTask = GetStatusesAsync(projectKey, cancellationToken);
        var typesTask = GetTypesAsync(cancellationToken);

        await Task.WhenAll(statusesTask, typesTask);

        var statuses = await statusesTask;
        var types = await typesTask;

        var typesById = types.DistinctBy(t => t.Id).ToDictionary(t => t.Id);

        return
        [
            ..statuses.Select(t =>
            {
                var globalType = typesById.GetValueOrDefault(t.Id);
                return new Type
                {
                    Self = globalType?.Self ?? t.Self,
                    Id = t.Id,
                    Description = globalType?.Description ?? t.Description,
                    IconUrl = globalType?.IconUrl ?? t.IconUrl,
                    Name = globalType?.Name ?? t.Name,
                    Subtask = globalType?.Subtask ?? t.Subtask,
                    AvatarId = globalType?.AvatarId ?? t.AvatarId,
                    HierarchyLevel = globalType?.HierarchyLevel ?? t.HierarchyLevel,
                    Statuses = t.Statuses
                };
            })
        ];
    }

    public async Task<ImmutableArray<User>> GetAssigneesAsync(string projectKey, CancellationToken cancellationToken = default) =>
        await cache.GetOrSetAsync($"jira:assignees:{projectKey}", async ct => await jiraClient.GetAssigneesAsync(projectKey, ct), new FusionCacheEntryOptions { Duration = _cacheOptions.Assignees }, cancellationToken);

    public async Task<ImmutableArray<Component>> GetComponentsAsync(string projectKey, CancellationToken cancellationToken = default) =>
        await cache.GetOrSetAsync($"jira:components:{projectKey}", async ct => await jiraClient.GetComponentsAsync(projectKey, ct), new FusionCacheEntryOptions { Duration = _cacheOptions.Components }, cancellationToken);

    public async Task<ImmutableArray<string>> GetLabelsAsync(CancellationToken cancellationToken = default) =>
        await cache.GetOrSetAsync("jira:labels:all", async ct => await jiraClient.GetLabelsAsync(ct), new FusionCacheEntryOptions { Duration = _cacheOptions.Labels }, cancellationToken);
    
    private async Task<ImmutableArray<Type>> GetTypesAsync(CancellationToken cancellationToken = default) =>
        await cache.GetOrSetAsync("jira:types", async ct => await jiraClient.GetTypesAsync(ct), new FusionCacheEntryOptions { Duration = _cacheOptions.Types }, cancellationToken);

    private async Task<ImmutableArray<Type>> GetStatusesAsync(string projectKey, CancellationToken cancellationToken = default) =>
        await cache.GetOrSetAsync($"jira:statuses:{projectKey}", async ct => await jiraClient.GetStatusesAsync(projectKey, ct), new FusionCacheEntryOptions { Duration = _cacheOptions.Types }, cancellationToken);
}