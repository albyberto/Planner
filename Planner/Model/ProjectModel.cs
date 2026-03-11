using System.Collections.Immutable;

namespace Planner.Model;

public record ProjectModel
{
    private readonly Lazy<Task<ImmutableList<UserModel>>> _assigneesLoader;
    private readonly Lazy<Task<ImmutableList<ComponentModel>>> _componentsLoader;
    private readonly Lazy<Task<ImmutableList<LabelModel>>> _labelsLoader;

    private readonly Lazy<Task<ImmutableList<TypeModel>>> _typesLoader;

    public ProjectModel(string key, AvatarsModel avatars,
        Func<Task<ImmutableList<TypeModel>>> typesLoader,
        Func<Task<ImmutableList<UserModel>>> assigneesLoader,
        Func<Task<ImmutableList<ComponentModel>>> componentsLoader,
        Func<Task<ImmutableList<LabelModel>>> labelsLoader)
    {
        Key = key;
        Avatars = avatars;

        _typesLoader = new(typesLoader);
        _assigneesLoader = new(assigneesLoader);
        _componentsLoader = new(componentsLoader);
        _labelsLoader = new(labelsLoader);
    }

    public string Key { get; }
    public AvatarsModel Avatars { get; }

    public Task<ImmutableList<UserModel>> GetAssigneesAsync() => _assigneesLoader.Value;
    public Task<ImmutableList<ComponentModel>> GetComponentsAsync() => _componentsLoader.Value;
    public Task<ImmutableList<LabelModel>> GetLabelsAsync() => _labelsLoader.Value;
    public Task<ImmutableList<TypeModel>> GetTypesAsync() => _typesLoader.Value;
}