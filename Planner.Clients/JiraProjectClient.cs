using System.Collections.Immutable;
using System.Net.Http.Json;
using System.Text.Json; // Aggiunto
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options; // Aggiunto
using Planner.Clients.Domain;
using Planner.Clients.Domain.Responses;
using Type = Planner.Clients.Domain.Type;

namespace Planner.Clients;

// 1. Aggiunto IOptionsMonitor nel costruttore
public class JiraProjectClient(HttpClient httpClient, IOptionsMonitor<JsonSerializerOptions> jsonOptions, ILogger<JiraProjectClient> logger)
{
    // 2. Estratte le opzioni centralizzate
    private readonly JsonSerializerOptions _jsonOptions = jsonOptions.Get("JiraSerializer");
    
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
                
                // 3. Aggiunte _jsonOptions qui
                var response = await httpClient.GetFromJsonAsync<ProjectResponse>(uri, _jsonOptions, cancellationToken);

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
            // Aggiunte _jsonOptions qui
            var response = await httpClient.GetFromJsonAsync<Type[]>("issuetype", _jsonOptions, cancellationToken);
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
            // Aggiunte _jsonOptions qui
            var response = await httpClient.GetFromJsonAsync<Type[]>($"project/{projectKey}/statuses", _jsonOptions, cancellationToken);
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
            // Aggiunte _jsonOptions qui
            var response = await httpClient.GetFromJsonAsync<User[]>($"user/assignable/search?project={projectKey}&maxResults=1000", _jsonOptions, cancellationToken);
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
            // Aggiunte _jsonOptions qui
            var response = await httpClient.GetFromJsonAsync<Component[]>($"project/{projectKey}/components", _jsonOptions, cancellationToken);
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
                
                // Aggiunte _jsonOptions qui
                var response = await httpClient.GetFromJsonAsync<LabelResponse>(uri, _jsonOptions, cancellationToken);

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