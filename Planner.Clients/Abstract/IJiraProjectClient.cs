using System.Collections.Immutable;
using Planner.Clients.Domain;
using Type = Planner.Clients.Domain.Type;

namespace Planner.Clients.Abstract;

public interface IJiraProjectClient
{
    Task<ImmutableArray<Project>> GetProjectsAsync(CancellationToken cancellationToken = default);
    Task<ImmutableArray<Type>> GetTypesAsync(CancellationToken cancellationToken = default);
    Task<ImmutableArray<Type>> GetStatusesAsync(string projectKey, CancellationToken cancellationToken = default);
    Task<ImmutableArray<User>> GetAssigneesAsync(string projectKey, CancellationToken cancellationToken = default);
    Task<ImmutableArray<Component>> GetComponentsAsync(string projectKey, CancellationToken cancellationToken = default);
    Task<ImmutableArray<string>> GetLabelsAsync(CancellationToken cancellationToken = default);
}