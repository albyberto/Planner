using System.Collections.Immutable;
using Planner.Infrastructure.Core.Abstract;
using Planner.Infrastructure.Domain;

namespace Planner.Facades;

public class MenuConfigFacade(IMenuConfigService service)
{
    public Task<ImmutableArray<MenuConfigItem>> GetAllAsync() => service.GetAllAsync();
    public Task<MenuConfigItem?> SaveAsync(MenuConfigItem item) => service.SaveAsync(item);
    public Task DeleteAsync(Guid id) => service.DeleteAsync(id);
}
