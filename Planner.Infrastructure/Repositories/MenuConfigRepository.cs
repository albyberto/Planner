using System.Collections.Immutable;
using Planner.Infrastructure.Data;
using Planner.Infrastructure.Documents;
using Planner.Infrastructure.Domain;
using Planner.Infrastructure.Domain.Abstract;
using Planner.Infrastructure.Mappers;

namespace Planner.Infrastructure.Repositories;

public class MenuConfigRepository(IDatabaseContext context) : IMenuConfigRepository
{
    private static readonly SemaphoreSlim Lock = new(1, 1);

    public async Task<MenuConfigItem?> GetAsync(Guid id) => (await LoadAsync()).SingleOrDefault(d => d.Id == id);

    public Task<ImmutableArray<MenuConfigItem>> GetAllAsync() => LoadAsync();

    public async Task<MenuConfigItem?> CreateAsync(MenuConfigItem item)
    {
        await Lock.WaitAsync();

        try
        {
            var documents = (await context.GetAsync<MenuConfigDocument>()).ToList();
            var document = item.ToDocument();

            if (document.Id == Guid.Empty) document = document with { Id = Guid.NewGuid() };

            documents.Add(document);

            await context.SaveChangesAsync(documents);

            return item with { Id = document.Id };
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

    public async Task<MenuConfigItem?> UpdateAsync(Guid id, MenuConfigItem item)
    {
        await Lock.WaitAsync();
        try
        {
            var documents = (await context.GetAsync<MenuConfigDocument>()).ToList();

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
            var documents = (await context.GetAsync<MenuConfigDocument>()).ToList();

            documents.RemoveAll(d => d.Id == id);

            await context.SaveChangesAsync(documents);
        }
        finally
        {
            Lock.Release();
        }
    }

    private async Task<ImmutableArray<MenuConfigItem>> LoadAsync()
    {
        await Lock.WaitAsync();

        try
        {
            var documents = await context.GetAsync<MenuConfigDocument>();
            return [..documents.Select(document => document.ToItem())];
        }
        finally
        {
            Lock.Release();
        }
    }
}
