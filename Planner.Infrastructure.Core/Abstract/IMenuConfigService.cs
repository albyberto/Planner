using System.Collections.Immutable;
using Planner.Infrastructure.Domain;

namespace Planner.Infrastructure.Core.Abstract;

public interface IMenuConfigService
{
    Task<ImmutableArray<MenuConfigItem>> GetAllAsync();
    Task<MenuConfigItem?> SaveAsync(MenuConfigItem item);
    Task DeleteAsync(Guid id);
}
