using System.Collections.Immutable;
using Planner.Infrastructure.Core.Abstract;
using Planner.Infrastructure.Domain;
using Planner.Infrastructure.Domain.Abstract;
using ZiggyCreatures.Caching.Fusion;

namespace Planner.Infrastructure.Core;

public class MenuConfigService(IMenuConfigRepository repository, IFusionCache cache) : IMenuConfigService
{
    private const string CacheKey = "all_menu_configs";

    public async Task<ImmutableArray<MenuConfigItem>> GetAllAsync()
    {
        return await cache.GetOrSetAsync<ImmutableArray<MenuConfigItem>>(
            CacheKey,
            async _ => 
            {
                var items = await repository.GetAllAsync();
                return [
                    ..items.OrderBy(config => config.ProjectKey, StringComparer.OrdinalIgnoreCase)
                ];
            }
        );
    }

    public async Task<MenuConfigItem?> SaveAsync(MenuConfigItem item)
    {
        var savedItem = item.Id == Guid.Empty 
            ? await repository.CreateAsync(item) 
            : await repository.UpdateAsync(item.Id, item);

        if (savedItem is null) return null;

        var cachedConfigs = await cache.GetOrDefaultAsync<ImmutableArray<MenuConfigItem>>(CacheKey);
        
        if (!cachedConfigs.IsDefaultOrEmpty)
        {
            var list = cachedConfigs.ToList();

            var existingIndex = list.FindIndex(c => c.Id == savedItem.Id);
            if (existingIndex >= 0) list[existingIndex] = savedItem;
            else list.Add(savedItem);

            var updatedCache = list
                .OrderBy(config => config.ProjectKey, StringComparer.OrdinalIgnoreCase)
                .ToImmutableArray();

            await cache.SetAsync(CacheKey, updatedCache);
        }
        else
        {
            await GetAllAsync();
        }

        return savedItem;
    }

    public async Task DeleteAsync(Guid id)
    {
        await repository.DeleteAsync(id);

        var cachedConfigs = await cache.GetOrDefaultAsync<ImmutableArray<MenuConfigItem>>(CacheKey);
        
        if (!cachedConfigs.IsDefaultOrEmpty)
        {
            var updatedCache = cachedConfigs.Where(c => c.Id != id).ToImmutableArray();
            await cache.SetAsync(CacheKey, updatedCache);
        }
    }
}
