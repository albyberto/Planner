using System.Collections.Immutable;
using Planner.Domain;
using Planner.Infrastructure.Clients;
using Planner.Model;

namespace Planner.Services;

public class ProjectsService(JiraFilterClient client)
{
    public async Task<ImmutableHashSet<ProjectModel>> GetAsync(CancellationToken cancellationToken = default)
    {
        var projects = await client.GetProjectsAsync(cancellationToken);

        return projects
            .Select(project => CreateProjectModel(project, cancellationToken))
            .ToImmutableHashSet();
    }

    public async Task<ProjectModel?> GetAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var projects = await client.GetProjectsAsync(cancellationToken);

        var project = projects
            .SingleOrDefault(p => p.Key.Equals(projectKey, StringComparison.OrdinalIgnoreCase));

        return project is not null
            ? CreateProjectModel(project, cancellationToken)
            : null;
    }

    private ProjectModel CreateProjectModel(Project project, CancellationToken cancellationToken)
    {
        var key = project.Key;

        return new(
            key,
            new(project.AvatarUrls),
            () => FetchTypesAsync(key, cancellationToken),
            () => FetchStatusesFromTypesAsync(key, cancellationToken),
            () => FetchAssigneesAsync(key, cancellationToken),
            () => FetchComponentsAsync(key, cancellationToken),
            () => FetchLabelsAsync(cancellationToken)
        );
    }

    private async Task<ImmutableHashSet<TypeModel>> FetchTypesAsync(string key, CancellationToken cancellationToken)
    {
        var types = await client.GetTypesAsync(key, cancellationToken);
        
        return types
            .Select(type => new TypeModel(type))
            .ToImmutableHashSet();
    }

    private async Task<ImmutableHashSet<StatusModel>> FetchStatusesFromTypesAsync(string key, CancellationToken cancellationToken)
    {
        var types = await FetchTypesAsync(key, cancellationToken);
        
        return types
            .SelectMany(type => type.Statuses)
            .DistinctBy(status => status.Name)
            .OrderBy(status => status.Name)
            .ToImmutableHashSet();
    }

    private async Task<ImmutableHashSet<UserModel>> FetchAssigneesAsync(string key, CancellationToken cancellationToken)
    {
        var assignees = await client.GetAssigneesAsync(key, cancellationToken);
        
        return assignees
            .Select(assignee => new UserModel(assignee))
            .ToImmutableHashSet();
    }

    private async Task<ImmutableHashSet<ComponentModel>> FetchComponentsAsync(string key, CancellationToken cancellationToken)
    {
        var components = await client.GetComponentsAsync(key, cancellationToken);
        
        return components
            .Select(component => new ComponentModel(component))
            .ToImmutableHashSet();
    }

    private async Task<ImmutableHashSet<LabelModel>> FetchLabelsAsync(CancellationToken cancellationToken)
    {
        var labels = await client.GetLabelsAsync(cancellationToken);
        
        return labels
            .Select(label => new LabelModel(label))
            .ToImmutableHashSet();
    }
}