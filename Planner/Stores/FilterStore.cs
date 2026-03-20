using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Planner.Model;

namespace Planner.Stores;

public class FilterStore
{
    private class FilterEntry
    {
        public BehaviorSubject<SearchCriteria> Subject { get; } = new(SearchCriteria.Empty);
        public DateTime LastFetched { get; set; } = DateTime.MinValue;
    }

    private readonly ConcurrentDictionary<Guid, FilterEntry> _entries = new();
    private readonly Subject<Emit<SearchCriteria>> _stream = new();

    public IObservable<SearchCriteria> Observe(Guid key) => _entries.TryGetValue(key, out var entry) 
        ? entry.Subject.AsObservable() 
        : Observable.Empty<SearchCriteria>();

    public IObservable<Emit<SearchCriteria>> ObserveGlobal() => _stream.AsObservable();

    public IReadOnlyList<Emit<SearchCriteria>> GetFiltersForPolling(TimeSpan threshold)
    {
        var now = DateTime.UtcNow;
        var result = new List<Emit<SearchCriteria>>();

        foreach (var kvp in _entries)
        {
            if (now - kvp.Value.LastFetched < threshold) continue;
            
            result.Add(new(kvp.Key, kvp.Value.Subject.Value));
                
            kvp.Value.LastFetched = now;
        }

        return result;
    }

    public void MarkAsFetched(Guid key)
    {
        if (_entries.TryGetValue(key, out var entry)) entry.LastFetched = DateTime.UtcNow;
    }
    
    public void Emit(Guid key, SearchCriteria searchCriteria)
    {
        if (_entries.TryGetValue(key, out var entry))
        {
            entry.Subject.OnNext(searchCriteria);
            entry.LastFetched = DateTime.UtcNow; 
        }
        
        _stream.OnNext(new(key, searchCriteria));
    }
    
    public void Register(Guid key) => _entries.TryAdd(key, new());
    
    public void UnRegister(Guid key)
    {
        if (_entries.TryRemove(key, out var entry)) entry.Subject.Dispose();
    }
}