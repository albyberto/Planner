using System.Collections.Immutable;

namespace Planner.Model;

public record ProjectModel
{
    private readonly Func<Task<ImmutableHashSet<TypeModel>>> _typesLoader;
    private readonly Func<Task<ImmutableHashSet<StatusModel>>> _statusesLoader;
    private readonly Func<Task<ImmutableHashSet<UserModel>>> _assigneesLoader;
    private readonly Func<Task<ImmutableHashSet<ComponentModel>>> _componentsLoader;
    private readonly Func<Task<ImmutableHashSet<LabelModel>>> _labelsLoader;

    // Campi di cache per i Task
    private Task<ImmutableHashSet<TypeModel>>? _typesTask;
    private Task<ImmutableHashSet<StatusModel>>? _statusesTask;
    private Task<ImmutableHashSet<UserModel>>? _assigneesTask;
    private Task<ImmutableHashSet<ComponentModel>>? _componentsTask;
    private Task<ImmutableHashSet<LabelModel>>? _labelsTask;

    public ProjectModel(
        string key, 
        AvatarsModel avatars,
        Func<Task<ImmutableHashSet<TypeModel>>> typesLoader,
        Func<Task<ImmutableHashSet<StatusModel>>> statusesLoader,
        Func<Task<ImmutableHashSet<UserModel>>> assigneesLoader,
        Func<Task<ImmutableHashSet<ComponentModel>>> componentsLoader,
        Func<Task<ImmutableHashSet<LabelModel>>> labelsLoader)
    {
        Key = key;
        Avatars = avatars;
        _typesLoader = typesLoader;
        _statusesLoader = statusesLoader;
        _assigneesLoader = assigneesLoader;
        _componentsLoader = componentsLoader;
        _labelsLoader = labelsLoader;
    }

    public string Key { get; }
    public AvatarsModel Avatars { get; }

    public Task<ImmutableHashSet<TypeModel>> GetIssueTypesAsync() => 
        _typesTask ??= _typesLoader();

    public Task<ImmutableHashSet<StatusModel>> GetStatusesAsync() => 
        _statusesTask ??= _statusesLoader();

    public Task<ImmutableHashSet<UserModel>> GetAssigneesAsync() => 
        _assigneesTask ??= _assigneesLoader();

    public Task<ImmutableHashSet<ComponentModel>> GetComponentsAsync() => 
        _componentsTask ??= _componentsLoader();

    public Task<ImmutableHashSet<LabelModel>> GetLabelsAsync() => 
        _labelsTask ??= _labelsLoader();
}