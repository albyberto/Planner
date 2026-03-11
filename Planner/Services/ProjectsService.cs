using System.Collections.Immutable;
using Planner.Domain;
using Planner.Infrastructure.Clients;
using Planner.Model;

namespace Planner.Services;

public class ProjectsService(JiraFilterClient client)
{
    /// <summary>
    /// Recupera tutti i progetti mappati per la UI.
    /// </summary>
    public async Task<IEnumerable<ProjectModel>> GetAsync(CancellationToken cancellationToken = default)
    {
        var projects = await client.GetProjectsAsync(cancellationToken);

        return projects.Select(project => CreateProjectModel(project, cancellationToken));
    }

    /// <summary>
    /// Recupera un singolo progetto tramite la sua Project Key.
    /// Restituisce null se il progetto non viene trovato.
    /// </summary>
    public async Task<ProjectModel?> GetAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var projects = await client.GetProjectsAsync(cancellationToken);

        var project = projects.FirstOrDefault(p => p.Key.Equals(projectKey, StringComparison.OrdinalIgnoreCase));

        return project is not null ? CreateProjectModel(project, cancellationToken) : null;
    }

    /// <summary>
    /// Metodo privato per centralizzare la logica di mapping da Domain a Model.
    /// </summary>
    private ProjectModel CreateProjectModel(Project project, CancellationToken cancellationToken)
    {
        var lazyTypes = new Lazy<Task<ImmutableList<TypeModel>>>(async () =>
        {
            var types = await client.GetTypesAsync(project.Key, cancellationToken);
            return types.Select(type => new TypeModel(type)).ToImmutableList();
        });

        var lazyAssignees = new Lazy<Task<ImmutableList<UserModel>>>(async () =>
        {
            var assignees = await client.GetAssigneesAsync(project.Key, cancellationToken);
            return assignees.Select(assignee => new UserModel(assignee)).ToImmutableList();
        });

        var lazyComponents = new Lazy<Task<ImmutableList<ComponentModel>>>(async () =>
        {
            var components = await client.GetComponentsAsync(project.Key, cancellationToken);
            return components.Select(component => new ComponentModel(component)).ToImmutableList();
        });

        var lazyLabels = new Lazy<Task<ImmutableList<LabelModel>>>(async () =>
        {
            var labels = await client.GetLabelsAsync(cancellationToken);
            return labels.Select(label => new LabelModel(label)).ToImmutableList();
        });

        return new(
            project.Key,
            new(project.AvatarUrls),

            () => lazyTypes.Value,
            () => lazyAssignees.Value,
            () => lazyComponents.Value,
            () => lazyLabels.Value
        );
    }
}