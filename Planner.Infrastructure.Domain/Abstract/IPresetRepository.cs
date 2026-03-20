using System.Collections.Immutable;

namespace Planner.Infrastructure.Domain.Abstract;

public interface IPresetRepository
{
    Task<PresetItem?> GetAsync(Guid id);
    Task<ImmutableArray<PresetItem>> GetAllAsync();
    Task<(ImmutableArray<PresetItem> Items, int TotalCount)> GetPagedAsync(int skip, int take);
    Task<PresetItem?> CreateAsync(PresetItem preset);
    Task<PresetItem?> UpdateAsync(Guid id, PresetItem preset);
    Task DeleteAsync(Guid id);
}