using System.Collections.Immutable;
using Planner.Infrastructure.Core.Abstract;
using Planner.Infrastructure.Domain;
using Planner.Infrastructure.Domain.Abstract;

namespace Planner.Infrastructure.Core;

public class PresetService(IPresetRepository repository) : IPresetService
{
    public async Task<ImmutableArray<PresetItem>> GetAllAsync() => 
        [
            ..(await repository.GetAllAsync())
            .OrderBy(preset => preset.Default ? 0 : 1)
            .ThenBy(preset => preset.Name, StringComparer.OrdinalIgnoreCase)
        ];

    public async Task<PresetItem?> SaveAsync(PresetItem item) =>
        item.Id == Guid.Empty ? await repository.CreateAsync(item) : await repository.UpdateAsync(item.Id, item);

    public Task DeleteAsync(Guid id) => repository.DeleteAsync(id);
}