using System.Collections.Immutable;
using Microsoft.Extensions.Options;
using Planner.Clients.Core;
using Planner.Extensions;
using Planner.Model;
using Planner.Options;

namespace Planner.Facades;

public class ProjectFacade(IOptions<JiraFilterOptions> options, ProjectService client)
{
    public async Task<ImmutableArray<ProjectModel>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        var projects = await client.GetProjectsAsync(cancellationToken);
        
        return  [
            ..projects
                .Select(p => new ProjectModel(p.Key, new(p.AvatarUrls)))
                .OrderBy(p => p.Key)];
    }

    public async Task<ImmutableArray<TypeModel>> GetTypesAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var types = await client.GetTypesAndStatusesAsync(projectKey, cancellationToken);
        return [
            ..types
                .Select(t => new TypeModel(t))
                .OrderBy(t => t.Name)
        ];
    }

    public async Task<ImmutableArray<StatusModel>> GetStatusesAsync(string projectKey, ImmutableHashSet<TypeModel> selectedTypes, CancellationToken cancellationToken = default)
    {
        var allTypes = await GetTypesAsync(projectKey, cancellationToken);
        var activeTypes = selectedTypes.IsEmpty ? allTypes : allTypes.Where(type => selectedTypes.Select(t => t.Name).Contains(type.Name));

        var statuses = activeTypes
            .SelectMany(type => type.Statuses)
            .DistinctBy(status => status.Name);
        
        return string.Equals(projectKey, options.Value.DefaultProject, StringComparison.OrdinalIgnoreCase)
            ? [..statuses.Sort()]
            : [..statuses.OrderBy(s => s.Name)];
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

    public async Task<ImmutableArray<LabelModel>> GetLabelsAsync(CancellationToken cancellationToken = default)
    {
        var labels = await client.GetLabelsAsync(cancellationToken);
    
        return  [
            ..labels
                .Select(l => new LabelModel(l))
                .OrderBy(l => l.Name)];
    }
}