using System.Collections.Immutable;
using System.Text.Json;
using Planner.Infrastructure.Documents;
using Planner.Infrastructure.Domain;
using Planner.Infrastructure.Domain.Abstract;
using Planner.Infrastructure.Mappers;

namespace Planner.Infrastructure.Repositories;

public class PresetRepository(string filePath) : IPresetRepository
{
    private static readonly SemaphoreSlim Lock = new(1, 1);
    
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

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
            var documents = await LoadInternalAsync();
            var document = item.ToDocument();
            
            documents.Add(document);
            await SaveInternalAsync(documents);
            
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
            var documents = await LoadInternalAsync();
        
            var index = documents.FindIndex(d => d.Id == id);

            if (index == -1) return null;
            
            var existingDocument = documents[index];
            
            var updatedDocument = existingDocument.Merge(item);
            
            documents[index] = updatedDocument;
            await SaveInternalAsync(documents);

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
            var documents = await LoadInternalAsync();
            documents.RemoveAll(d => d.Id == id);
            await SaveInternalAsync(documents);
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
            return [..(await LoadInternalAsync()).Select(document => document.ToItem())];
        }
        finally
        {
            Lock.Release();
        }
    }

    private async Task<List<PresetDocument>> LoadInternalAsync()
    {
        if (!File.Exists(filePath)) return [];

        await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var docs = await JsonSerializer.DeserializeAsync<List<PresetDocument>>(stream, JsonOptions);
        return docs ?? [];
    }

    private async Task SaveInternalAsync(List<PresetDocument> documents)
    {
        await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(stream, documents, JsonOptions);
    }
}