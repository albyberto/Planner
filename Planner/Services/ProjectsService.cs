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

    public async Task<ImmutableArray<ProjectModel>> GetProjectsPageAsync(uint skip, uint take, CancellationToken cancellationToken = default)
    {
        var projects = await client.GetProjectsAsync(skip, take, cancellationToken);
        return [
            ..projects
                .Select(p => new ProjectModel(p.Key, new(p.AvatarUrls)))
        ];
    }

    public async Task<ImmutableArray<TypeModel>> GetTypesAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var types = await client.GetTypesAsync(projectKey, cancellationToken);
        return [
            ..types
                .Select(t => new TypeModel(t))
                .OrderBy(t => t.Name)
        ];
    }

    public async Task<ImmutableArray<StatusModel>> GetStatusesAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var types = await client.GetTypesAsync(projectKey, cancellationToken);
        return [
            ..types
                .SelectMany(type => type.Statuses)
                .DistinctBy(status => status.Name)
                .Select(s => new StatusModel(s))
                .OrderBy(s => s.Name)
        ];
    }

    public async Task<ImmutableArray<UserModel>> GetAssigneesAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var assignees = await client.GetAssigneesAsync(projectKey, cancellationToken);
        return [
            ..assignees
                .Select(a => new UserModel(a))
                .OrderBy(a => a.DisplayName)
        ];
    }

    public async Task<ImmutableArray<ComponentModel>> GetComponentsAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var components = await client.GetComponentsAsync(projectKey, cancellationToken);
        return [
            ..components
                .Select(c => new ComponentModel(c))
                .OrderBy(c => c.Name)
        ];
    }

    public async Task<ImmutableArray<LabelModel>> GetLabelsPageAsync(uint skip, uint take, CancellationToken cancellationToken = default)
    {
        var labels = await client.GetLabelsAsync(skip, take, cancellationToken);
        return [
            ..labels
                .Select(l => new LabelModel(l))
        ];
    }
}