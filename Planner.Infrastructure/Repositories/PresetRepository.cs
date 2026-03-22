using System.Collections.Immutable;
using Planner.Infrastructure.Data;
using Planner.Infrastructure.Documents;
using Planner.Infrastructure.Domain;
using Planner.Infrastructure.Domain.Abstract;
using Planner.Infrastructure.Mappers;

namespace Planner.Infrastructure.Repositories;

public class PresetRepository(IDatabaseContext context) : IPresetRepository
{
    private static readonly SemaphoreSlim Lock = new(1, 1);

    public async Task<PresetItem?> GetAsync(Guid id) => (await LoadAsync()).SingleOrDefault(document => document.Id == id);

    public Task<ImmutableArray<PresetItem>> GetAllAsync() => LoadAsync();

    public async Task<(ImmutableArray<PresetItem> Items, int TotalCount)> GetPagedAsync(int skip, int take)
    {
        var documents = await LoadAsync();
        var paged = documents.Skip((skip + 1) * take).Take(take).ToImmutableArray();

        return (paged, documents.Length);
    }

    public async Task<PresetItem?> CreateAsync(PresetItem item)
    {
        await Lock.WaitAsync();

        try
        {
            var documents = (await context.GetAsync<PresetDocument>()).ToList();
            var document = item.ToDocument();

            if (document.Id == Guid.Empty) document = document with { Id = Guid.NewGuid() };

            documents.Add(document);

            await context.SaveChangesAsync(documents);

            return item;
        }
        catch
        {
            return null;
        }
        finally
        {
            Lock.Release();
        }
    }

    public async Task<PresetItem?> UpdateAsync(Guid id, PresetItem item)
    {
        await Lock.WaitAsync();
        try
        {
            var documents = (await context.GetAsync<PresetDocument>()).ToList();

            var index = documents.FindIndex(d => d.Id == id);

            if (index == -1) return null;

            var existingDocument = documents[index];

            var updatedDocument = existingDocument.Merge(item);

            documents[index] = updatedDocument;

            await context.SaveChangesAsync(documents);

            return item;
        }
        finally
        {
            Lock.Release();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        await Lock.WaitAsync();
        try
        {
            var documents = (await context.GetAsync<PresetDocument>()).ToList();

            documents.RemoveAll(d => d.Id == id);

            await context.SaveChangesAsync(documents);
        }
        finally
        {
            Lock.Release();
        }
    }

    private async Task<ImmutableArray<PresetItem>> LoadAsync()
    {
        await Lock.WaitAsync();

        try
        {
            var documents = await context.GetAsync<PresetDocument>();
            return [..documents.Select(document => document.ToItem())];
        }
        finally
        {
            Lock.Release();
        }
    }
}