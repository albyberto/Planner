using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Planner.Model;

namespace Planner.Stores;

public class FilterStore
{
    private class FilterEntry
    {
        public BehaviorSubject<IssueSearchCriteria> Subject { get; } = new(IssueSearchCriteria.Empty);
        public DateTime LastFetched { get; set; } = DateTime.MinValue;
    }

    private readonly ConcurrentDictionary<Guid, FilterEntry> _entries = new();
    private readonly Subject<Emit<IssueSearchCriteria>> _stream = new();

    public IObservable<IssueSearchCriteria> Observe(Guid key) => _entries.TryGetValue(key, out var entry) 
        ? entry.Subject.AsObservable() 
        : Observable.Empty<IssueSearchCriteria>();

    public IObservable<Emit<IssueSearchCriteria>> ObserveGlobal() => _stream.AsObservable();

    public IReadOnlyList<Emit<IssueSearchCriteria>> GetFiltersForPolling(TimeSpan threshold)
    {
        var now = DateTime.UtcNow;
        var result = new List<Emit<IssueSearchCriteria>>();

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
    
    public void Emit(Guid key, IssueSearchCriteria issueSearchCriteria)
    {
        if (_entries.TryGetValue(key, out var entry))
        {
            entry.Subject.OnNext(issueSearchCriteria);
            entry.LastFetched = DateTime.UtcNow; 
        }
        
        _stream.OnNext(new(key, issueSearchCriteria));
    }
    
    public void Register(Guid key) => _entries.TryAdd(key, new());
    
    public void UnRegister(Guid key)
    {
        if (_entries.TryRemove(key, out var entry)) entry.Subject.Dispose();
    }
}