using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Planner.Model;

namespace Planner.Stores;

public class IssueStore
{
    private readonly ConcurrentDictionary<Guid, BehaviorSubject<ImmutableArray<DashboardIssueModel>>> _store = new();
    private readonly ImmutableArray<DashboardIssueModel> _initialValue = ImmutableArray<DashboardIssueModel>.Empty;

    public IObservable<ImmutableArray<DashboardIssueModel>> Observe(Guid key) => GetOrCreate(key).AsObservable();

    public void Emit(Guid key, ImmutableArray<DashboardIssueModel> data) => GetOrCreate(key).OnNext(data);

    public void Unregister(Guid key)
    {
        if (!_store.TryRemove(key, out var subject)) return;
        
        subject.OnCompleted();
        subject.Dispose();
    }

    private BehaviorSubject<ImmutableArray<DashboardIssueModel>> GetOrCreate(Guid key) => _store.GetOrAdd(key, static (_, initVal) => new(initVal), _initialValue);
}