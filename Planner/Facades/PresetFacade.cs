using System.Collections.Immutable;
using Planner.Infrastructure.Core.Abstract;
using Planner.Infrastructure.Domain;

namespace Planner.Facades;

public class PresetFacade(IPresetService service)
{
    public Task<PresetItem?> GetDefaultAsync() => service.GetDefaultAsync();
    
    public async Task<ImmutableArray<PresetItem>> GetAllAsync()
    {
        var presets = await service.GetAllAsync();
        return [.. presets.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)];
    }

    public Task<PresetItem?> SaveAsync(PresetItem item) 
        => service.SaveAsync(item);

    public Task DeleteAsync(Guid id) => service.DeleteAsync(id);
}