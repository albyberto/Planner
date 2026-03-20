using System.Collections.Immutable;
using Planner.Infrastructure.Domain;
using Planner.Infrastructure.Domain.Abstract;

namespace Planner.Infrastructure.Core;

public class PresetService(IPresetRepository repository) : IPresetService
{
    public Task<ImmutableArray<PresetItem>> GetAllAsync() => repository.GetAllAsync();

    public async Task<PresetItem?> SaveAsync(PresetItem item) =>
        item.Id == Guid.Empty ? await repository.CreateAsync(item) : await repository.UpdateAsync(item.Id, item);

    public Task DeleteAsync(Guid id) => repository.DeleteAsync(id);
}