using System.Collections.Immutable;
using Planner.Infrastructure.Core.Abstract;
using Planner.Infrastructure.Domain;
using Planner.Infrastructure.Domain.Abstract;
using ZiggyCreatures.Caching.Fusion;

namespace Planner.Infrastructure.Core;

public class PresetService(IPresetRepository repository, IFusionCache cache) : IPresetService
{
    private const string CacheKey = "all_presets";

    public async Task<PresetItem?> GetDefaultAsync()
    {
        var presets = await GetAllAsync();
        return presets.FirstOrDefault(p => p.Default);
    }

    public async Task<ImmutableArray<PresetItem>> GetAllAsync()
    {
        return await cache.GetOrSetAsync<ImmutableArray<PresetItem>>(
            CacheKey,
            async _ => 
            {
                var items = await repository.GetAllAsync();
                return [
                    ..items
                        .OrderBy(preset => preset.Default ? 0 : 1)
                        .ThenBy(preset => preset.Name, StringComparer.OrdinalIgnoreCase)
                ];
            }
        );
    }

    public async Task<PresetItem?> SaveAsync(PresetItem item)
    {
        PresetItem? updatedOldDefault = null;

        if (item.Default)
        {
            var allPresets = await GetAllAsync();
            var previousDefault = allPresets.FirstOrDefault(p => p.Default && p.Id != item.Id);

            if (previousDefault is not null)
            {
                updatedOldDefault = previousDefault with { Default = false };
                await repository.UpdateAsync(updatedOldDefault.Id, updatedOldDefault);
            }
        }

        var savedItem = item.Id == Guid.Empty 
            ? await repository.CreateAsync(item) 
            : await repository.UpdateAsync(item.Id, item);

        if (savedItem is null) return null;

        var cachedPresets = await cache.GetOrDefaultAsync<ImmutableArray<PresetItem>>(CacheKey);
        
        if (!cachedPresets.IsDefaultOrEmpty)
        {
            var list = cachedPresets.ToList();

            if (updatedOldDefault is not null)
            {
                var oldIndex = list.FindIndex(p => p.Id == updatedOldDefault.Id);
                if (oldIndex >= 0) list[oldIndex] = updatedOldDefault;
            }

            var existingIndex = list.FindIndex(p => p.Id == savedItem.Id);
            if (existingIndex >= 0) list[existingIndex] = savedItem;
            else list.Add(savedItem);

            var updatedCache = list
                .OrderBy(preset => preset.Default ? 0 : 1)
                .ThenBy(preset => preset.Name, StringComparer.OrdinalIgnoreCase)
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

        var cachedPresets = await cache.GetOrDefaultAsync<ImmutableArray<PresetItem>>(CacheKey);
        
        if (!cachedPresets.IsDefaultOrEmpty)
        {
            var updatedCache = cachedPresets.Where(p => p.Id != id).ToImmutableArray();
            await cache.SetAsync(CacheKey, updatedCache);
        }
    }
}