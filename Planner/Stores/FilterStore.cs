using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Planner.Model;

namespace Planner.Stores;

public class FilterStore
{
    private readonly ConcurrentDictionary<Guid, BehaviorSubject<SearchCriteria>> _filters = new();
    private readonly Subject<(Guid Key, SearchCriteria Criteria)> _stream = new();

    public IObservable<SearchCriteria> Observe(Guid key) => GetOrCreate(key).AsObservable();

    public IObservable<(Guid Key, SearchCriteria Criteria)> ObserveUpdates() => _stream.AsObservable();

    public void Emit(Guid key, SearchCriteria criteria)
    {
        GetOrCreate(key).OnNext(criteria);
        _stream.OnNext((key, criteria));
    }

    public IReadOnlyDictionary<Guid, SearchCriteria> GetActiveFilters() => 
        _filters.ToDictionary(k => k.Key, v => v.Value.Value);

    public void Unregister(Guid key)
    {
        if (!_filters.TryRemove(key, out var subject)) return;
        
        subject.OnCompleted();
        subject.Dispose();
    }

    private BehaviorSubject<SearchCriteria> GetOrCreate(Guid key) =>
        _filters.GetOrAdd(key, _ => new(SearchCriteria.Empty));
}