using System.Collections.Immutable;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Planner.Domain;
using Planner.Domain.Responses;
using Planner.Infrastructure.Options;
using ZiggyCreatures.Caching.Fusion;
using Type = Planner.Domain.Type;

namespace Planner.Infrastructure.Clients;

public class JiraFilterClient(HttpClient httpClient, IFusionCache cache, IOptions<CacheOptions> cacheOptions, ILogger<JiraFilterClient> logger)
{
    private readonly CacheOptions _cacheOptions = cacheOptions.Value;

    public async Task<ImmutableArray<Project>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "jira:projects:all";

        return await cache.GetOrSetAsync(cacheKey, async ct =>
        {
            const int maxResults = 100;

            var isLast = false;
            var startAt = 0;

            List<Project> accumulator = [];

            try
            {
                while (!isLast)
                {
                    var uri = $"project/search?startAt={startAt}&maxResults={maxResults}";
                    var response = await httpClient.GetFromJsonAsync<ProjectResponse>(uri, ct);

                    if (response is null) break;
                    if (response.Projects.Any()) accumulator.AddRange(response.Projects);

                    isLast = response.IsLast;
                    startAt += maxResults;

                    if (startAt >= response.Total) isLast = true;
                }

                return accumulator.DistinctBy(project => project.Id).ToImmutableArray();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error fetching all paginated projects from Jira.");
                throw;
            }
        }, new FusionCacheEntryOptions { Duration = _cacheOptions.Projects }, cancellationToken);
    }

private async Task<ImmutableArray<Type>> GetTypesAsync(CancellationToken cancellationToken = default) =>
    await cache.GetOrSetAsync("jira:types", async ct =>
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<Type[]>("issuetype", ct);
            return response?.ToImmutableArray() ?? [];
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error fetching global issue types."); // Log corretto
            throw;
        }
    }, new FusionCacheEntryOptions { Duration = _cacheOptions.Types }, cancellationToken);

private async Task<ImmutableArray<Type>> GetStatusesAsync(string projectKey, CancellationToken cancellationToken = default) =>
    await cache.GetOrSetAsync($"jira:statuses:{projectKey}", async ct =>
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<Type[]>($"project/{projectKey}/statuses", ct);
            return response?.ToImmutableArray() ?? []; // Controllo null semplificato
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error fetching statuses for project {Project}", projectKey); // Log corretto
            throw;
        }
    }, new FusionCacheEntryOptions { Duration = _cacheOptions.Types }, cancellationToken);

public async Task<ImmutableArray<Type>> GetTypesAndStatusesAsync(string projectKey, CancellationToken cancellationToken = default)
{
    // 1. Avvia entrambe le task in parallelo
    var statusesTask = GetStatusesAsync(projectKey, cancellationToken);
    var typesTask = GetTypesAsync(cancellationToken);

    await Task.WhenAll(statusesTask, typesTask);

    var statuses = await statusesTask;
    var types = await typesTask;

    var typesById = types.DistinctBy(t => t.Id).ToDictionary(t => t.Id);

    return [
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
        await cache.GetOrSetAsync($"jira:assignees:{projectKey}", async ct =>
        {
            try
            {
                var response = await httpClient.GetFromJsonAsync<User[]>($"user/assignable/search?project={projectKey}&maxResults=1000", ct);
                return (response ?? []).Where(user => !string.IsNullOrWhiteSpace(user.EmailAddress)).DistinctBy(user => user.AccountId).ToImmutableArray();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error fetching assignees for project {Project}", projectKey);
                throw;
            }
        }, new FusionCacheEntryOptions { Duration = _cacheOptions.Assignees }, cancellationToken);

    public async Task<ImmutableArray<Component>> GetComponentsAsync(string projectKey, CancellationToken cancellationToken = default) =>
        await cache.GetOrSetAsync($"jira:components:{projectKey}", async ct =>
        {
            try
            {
                var response = await httpClient.GetFromJsonAsync<Component[]>($"project/{projectKey}/components", ct);
                return (response ?? []).DistinctBy(component => component.Id).ToImmutableArray();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error fetching components for project {Project}", projectKey);
                throw;
            }
        }, new FusionCacheEntryOptions { Duration = _cacheOptions.Components }, cancellationToken);

    public async Task<ImmutableArray<string>> GetLabelsAsync(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "jira:labels:all";

        return await cache.GetOrSetAsync(cacheKey, async ct =>
        {
            const int maxResults = 100;

            var isLast = false;
            var startAt = 0;

            List<string> accumulator = [];

            try
            {
                while (!isLast)
                {
                    var uri = $"label?startAt={startAt}&maxResults={maxResults}";
                    var response = await httpClient.GetFromJsonAsync<LabelResponse>(uri, ct);

                    if (response == null) break;
                    if (response.Values.Any()) accumulator.AddRange(response.Values);

                    isLast = response.IsLast;
                    startAt += maxResults;

                    if (response.Total > 0 && startAt >= response.Total) isLast = true;
                }

                return accumulator.Distinct().ToImmutableArray();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Errore durante il fetch di tutte le labels paginate da Jira.");
                throw;
            }
        }, new FusionCacheEntryOptions { Duration = _cacheOptions.Labels }, cancellationToken);
    }
}