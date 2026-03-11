using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Planner.Model;

namespace Planner.Stores;

public class FilterStore
{
    private readonly ConcurrentDictionary<Guid, Subject<FilterModel>> _subjects = new();

    public IObservable<FilterModel> Observe(Guid key) => _subjects.TryGetValue(key, out var subject) 
        ? subject.AsObservable() 
        : Observable.Empty<FilterModel>();
    
    public void Emit(Guid key, FilterModel filterModel)
    {
        if (_subjects.TryGetValue(key, out var subject))
        {
            subject.OnNext(filterModel);
        }
    }
    
    public void Register(Guid key) => _subjects.TryAdd(key, new());
    
    public void UnRegister(Guid key)
    {
        if (_subjects.TryRemove(key, out var subject))
        {
            subject.Dispose();
        }
    }
}