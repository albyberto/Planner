using System.Collections.Immutable;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Planner.Clients.Domain;
using Planner.Clients.Domain.Responses;
using Type = Planner.Clients.Domain.Type;

namespace Planner.Clients;

public class JiraProjectClient(HttpClient httpClient, ILogger<JiraProjectClient> logger)
{
    public async Task<ImmutableArray<Project>> GetProjectsAsync(CancellationToken cancellationToken = default)
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
                var response = await httpClient.GetFromJsonAsync<ProjectResponse>(uri, cancellationToken);

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
    }

    public async Task<ImmutableArray<Type>> GetTypesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<Type[]>("issuetype", cancellationToken);
            return response?.ToImmutableArray() ?? [];
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error fetching global issue types.");
            throw;
        }
    }

    public async Task<ImmutableArray<Type>> GetStatusesAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<Type[]>($"project/{projectKey}/statuses", cancellationToken);
            return response?.ToImmutableArray() ?? [];
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error fetching statuses for project {Project}", projectKey);
            throw;
        }
    }

    public async Task<ImmutableArray<User>> GetAssigneesAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<User[]>($"user/assignable/search?project={projectKey}&maxResults=1000", cancellationToken);
            return (response ?? []).Where(user => !string.IsNullOrWhiteSpace(user.EmailAddress)).DistinctBy(user => user.AccountId).ToImmutableArray();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error fetching assignees for project {Project}", projectKey);
            throw;
        }
    }

    public async Task<ImmutableArray<Component>> GetComponentsAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<Component[]>($"project/{projectKey}/components", cancellationToken);
            return (response ?? []).DistinctBy(component => component.Id).ToImmutableArray();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error fetching components for project {Project}", projectKey);
            throw;
        }
    }

    public async Task<ImmutableArray<string>> GetLabelsAsync(CancellationToken cancellationToken = default)
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
                var response = await httpClient.GetFromJsonAsync<LabelResponse>(uri, cancellationToken);

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
    }
}