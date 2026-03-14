using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Planner.Model;

namespace Planner.Stores;

public class FilterStore
{
    private class FilterEntry
    {
        public BehaviorSubject<IssuesSearchCriteria> Subject { get; } = new(IssuesSearchCriteria.Empty);
        public DateTime LastFetched { get; set; } = DateTime.MinValue;
    }

    private readonly ConcurrentDictionary<Guid, FilterEntry> _entries = new();
    private readonly Subject<Emit<IssuesSearchCriteria>> _stream = new();

    public IObservable<IssuesSearchCriteria> Observe(Guid key) => _entries.TryGetValue(key, out var entry) 
        ? entry.Subject.AsObservable() 
        : Observable.Empty<IssuesSearchCriteria>();

    public IObservable<Emit<IssuesSearchCriteria>> ObserveGlobal() => _stream.AsObservable();

    public IReadOnlyList<Emit<IssuesSearchCriteria>> GetFiltersForPolling(TimeSpan threshold)
    {
        var now = DateTime.UtcNow;
        var result = new List<Emit<IssuesSearchCriteria>>();

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
    
    public void Emit(Guid key, IssuesSearchCriteria issuesSearchCriteria)
    {
        if (_entries.TryGetValue(key, out var entry))
        {
            entry.Subject.OnNext(issuesSearchCriteria);
            entry.LastFetched = DateTime.UtcNow; 
        }
        
        _stream.OnNext(new(key, issuesSearchCriteria));
    }
    
    public void Register(Guid key) => _entries.TryAdd(key, new());
    
    public void UnRegister(Guid key)
    {
        if (_entries.TryRemove(key, out var entry)) entry.Subject.Dispose();
    }
}