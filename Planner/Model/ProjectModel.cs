using System.Collections.Immutable;
using Planner.Domain;

namespace Planner.Model;

public record ProjectModel
{
    public string Key { get; init; }
    
    // Le property Lazy ora contengono i nostri nuovi Model, non più il dominio
    private readonly Lazy<Task<ImmutableList<StatusModel>>> _statusesLoader;
    private readonly Lazy<Task<ImmutableList<UserModel>>> _assigneesLoader;
    private readonly Lazy<Task<ImmutableList<ComponentModel>>> _componentsLoader;
    private readonly Lazy<Task<ImmutableList<LabelModel>>> _labelsLoader;
    private readonly Lazy<Task<ImmutableList<IssueTypeModel>>> _issueTypesLoader;

    public ProjectModel(
        string key,
        Func<Task<ImmutableList<Status>>> loadStatuses,
        Func<Task<ImmutableList<User>>> loadAssignees,
        Func<Task<ImmutableList<Component>>> loadComponents,
        Func<Task<ImmutableList<string>>> loadLabels,
        Func<Task<ImmutableList<IssueType>>> loadIssueTypes)
    {
        Key = key;
        
        _statusesLoader = new(async () => 
        {
            var data = await loadStatuses();
            return data.Select(status => new StatusModel(status)).ToImmutableList();
        });

        _assigneesLoader = new(async () => 
        {
            var data = await loadAssignees();
            return data.Select(user => new UserModel(user)).ToImmutableList();
        });

        _componentsLoader = new(async () => 
        {
            var data = await loadComponents();
            return data.Select(component => new ComponentModel(component)).ToImmutableList();
        });

        _labelsLoader = new(async () => 
        {
            var data = await loadLabels();
            return data.Select(label => new LabelModel(label)).ToImmutableList();
        });

        _issueTypesLoader = new(async () => 
        {
            var data = await loadIssueTypes();
            return data.Select(type => new IssueTypeModel(type)).ToImmutableList();
        });
    }
    
    public Task<ImmutableList<StatusModel>> GetStatusesAsync() => _statusesLoader.Value;
    public Task<ImmutableList<UserModel>> GetAssigneesAsync() => _assigneesLoader.Value;
    public Task<ImmutableList<ComponentModel>> GetComponentsAsync() => _componentsLoader.Value;
    public Task<ImmutableList<LabelModel>> GetLabelsAsync() => _labelsLoader.Value;
    public Task<ImmutableList<IssueTypeModel>> GetIssueTypesAsync() => _issueTypesLoader.Value;
}