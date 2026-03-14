using System.Collections.Immutable;
using Planner.Infrastructure.Clients;
using Planner.Model;

namespace Planner.Services;

public class ProjectsService(JiraFilterClient client)
{
    public async Task<ProjectModel?> GetSingleProjectAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var project = await client.GetProjectAsync(projectKey, cancellationToken);
        return project is not null ? new ProjectModel(project.Key, new AvatarsModel(project.AvatarUrls)) : null;
    }

    // Usato da MudVirtualize per scrollare i progetti
    public async Task<ImmutableArray<ProjectModel>> GetProjectsPageAsync(uint skip, uint take, CancellationToken cancellationToken = default)
    {
        var projects = await client.GetProjectsAsync(skip, take, cancellationToken);
        return projects.Select(p => new ProjectModel(p.Key, new AvatarsModel(p.AvatarUrls))).ToImmutableArray();
    }

    public async Task<ImmutableArray<TypeModel>> GetTypesAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var types = await client.GetTypesAsync(projectKey, cancellationToken);
        return types.Select(t => new TypeModel(t.Id, t.Name)).ToImmutableArray();
    }

    public async Task<ImmutableArray<StatusModel>> GetStatusesAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var types = await client.GetTypesAsync(projectKey, cancellationToken);
        return types
            .SelectMany(type => type.Statuses)
            .DistinctBy(status => status.Name)
            .OrderBy(status => status.Name) // Ordinamento veloce grazie ad Array
            .Select(s => new StatusModel(s.Name))
            .ToImmutableArray();
    }

    public async Task<ImmutableArray<UserModel>> GetAssigneesAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var assignees = await client.GetAssigneesAsync(projectKey, cancellationToken);
        return assignees.Select(a => new UserModel(a.AccountId, a.DisplayName, a.EmailAddress!)).ToImmutableArray();
    }

    public async Task<ImmutableArray<ComponentModel>> GetComponentsAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var components = await client.GetComponentsAsync(projectKey, cancellationToken);
        return components.Select(c => new ComponentModel(c.Id, c.Name)).ToImmutableArray();
    }

    // Usato da MudVirtualize per scrollare le etichette
    public async Task<ImmutableArray<LabelModel>> GetLabelsPageAsync(uint skip, uint take, CancellationToken cancellationToken = default)
    {
        var labels = await client.GetLabelsAsync(skip, take, cancellationToken);
        return labels.Select(l => new LabelModel(l)).ToImmutableArray();
    }
}