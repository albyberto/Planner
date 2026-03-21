using System.Collections.Immutable;
using Planner.Infrastructure.Domain;

namespace Planner.Infrastructure.Core.Abstract;

public interface IPresetService
{
    Task<ImmutableArray<PresetItem>> GetAllAsync();
    Task<PresetItem?> SaveAsync(PresetItem item);
    Task DeleteAsync(Guid id);
}