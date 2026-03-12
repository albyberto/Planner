using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Planner.Model;

namespace Planner.Stores;

public class FilterStore
{
    private readonly ConcurrentDictionary<Guid, BehaviorSubject<IssueSearchCriteria>> _subjects = new();
    private readonly Subject<Emit<IssueSearchCriteria>> _stream = new();

    public IObservable<IssueSearchCriteria> Observe(Guid key) => _subjects.TryGetValue(key, out var subject) 
        ? subject.AsObservable() 
        : Observable.Empty<IssueSearchCriteria>();

    public IObservable<Emit<IssueSearchCriteria>> ObserveGlobal() => _stream.AsObservable();
    
    public void Emit(Guid key, IssueSearchCriteria issueSearchCriteria)
    {
        if (_subjects.TryGetValue(key, out var subject)) subject.OnNext(issueSearchCriteria);
        
        _stream.OnNext(new(key, issueSearchCriteria));
    }
    
    public void Register(Guid key) => _subjects.TryAdd(key, new(IssueSearchCriteria.Empty));
    
    public void UnRegister(Guid key)
    {
        if (_subjects.TryRemove(key, out var subject))
        {
            subject.Dispose();
        }
    }
}