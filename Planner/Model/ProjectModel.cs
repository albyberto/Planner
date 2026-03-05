using System.Collections.Immutable;
using Planner.Domain;

namespace Planner.Model;

public record ProjectModel
{
    public string Key { get; }
    
    private readonly Lazy<Task<Project>> _loadProject;
    private readonly Lazy<Task<ImmutableList<Status>>> _statusesLoader;
    private readonly Lazy<Task<ImmutableList<User>>> _assigneesLoader;
    private readonly Lazy<Task<ImmutableList<Component>>> _componentsLoader;
    private readonly Lazy<Task<ImmutableList<string>>> _labelsLoader;
    private readonly Lazy<Task<ImmutableList<IssueType>>> _issueTypesLoader;

    public ProjectModel(string key, Func<Task<Project>> loadProject, Func<Task<ImmutableList<Status>>> loadStatuses, Func<Task<ImmutableList<User>>> loadAssignees, Func<Task<ImmutableList<Component>>> loadComponents, Func<Task<ImmutableList<string>>> loadLabels, Func<Task<ImmutableList<IssueType>>> loadIssueTypes)
    {
        Key = key;
    
        _loadProject = new(loadProject);
        _statusesLoader = new(loadStatuses);
        _assigneesLoader = new(loadAssignees);
        _componentsLoader = new(loadComponents);
        _labelsLoader = new(loadLabels);
        _issueTypesLoader = new(loadIssueTypes);
    }

    public Task<Project> GetProjectAsync() => _loadProject.Value;
    public Task<ImmutableList<Status>> GetStatusesAsync() => _statusesLoader.Value;
    public Task<ImmutableList<User>> GetAssigneesAsync() => _assigneesLoader.Value;
    public Task<ImmutableList<Component>> GetComponentsAsync() => _componentsLoader.Value;
    public Task<ImmutableList<string>> GetLabelsAsync() => _labelsLoader.Value;
    public Task<ImmutableList<IssueType>> GetIssueTypesAsync() => _issueTypesLoader.Value;
}