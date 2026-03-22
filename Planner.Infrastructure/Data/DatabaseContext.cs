using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Planner.Infrastructure.Data;

public class JsonDatabaseContext : IDatabaseContext
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly string _databaseDirectory;

    public JsonDatabaseContext(IConfiguration configuration)
    {
        _databaseDirectory = configuration.GetConnectionString("DefaultConnection") ?? throw new NullReferenceException("DefaultConnection (Cartella Database) non trovata.");

        if (!Directory.Exists(_databaseDirectory)) Directory.CreateDirectory(_databaseDirectory);
    }

    public async Task<IEnumerable<T>> GetAsync<T>() where T : class
    {
        var filePath = GetFilePath<T>();

        if (!File.Exists(filePath)) return [];

        await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var items = await JsonSerializer.DeserializeAsync<List<T>>(stream, JsonOptions);

        return items ?? [];
    }

    public async Task SaveChangesAsync<T>(IEnumerable<T> entities) where T : class
    {
        var filePath = GetFilePath<T>();

        await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(stream, entities, JsonOptions);
    }

    private string GetFilePath<T>() => Path.Combine(_databaseDirectory, $"{typeof(T).Name}.json");
}