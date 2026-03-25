using System.Collections.Immutable;

namespace Planner.Infrastructure.Domain.Abstract;

public interface IMenuConfigRepository
{
    Task<MenuConfigItem?> GetAsync(Guid id);
    Task<ImmutableArray<MenuConfigItem>> GetAllAsync();
    Task<MenuConfigItem?> CreateAsync(MenuConfigItem item);
    Task<MenuConfigItem?> UpdateAsync(Guid id, MenuConfigItem item);
    Task DeleteAsync(Guid id);
}
