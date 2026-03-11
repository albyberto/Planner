using Microsoft.Extensions.Caching.Distributed;

namespace Planner.Infrastructure.Options;

public class CacheOptions
{
    public const string SectionName = nameof(CacheOptions);
    
    public DistributedCacheEntryOptions Projects { get; init; } = new();
    public DistributedCacheEntryOptions Types { get; init; } = new();
    public DistributedCacheEntryOptions Assignees { get; init; } = new();
    public DistributedCacheEntryOptions Components { get; init; } = new();
    public DistributedCacheEntryOptions Labels { get; init; } = new();
}