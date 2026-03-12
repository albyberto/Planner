using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Planner.Stores;

public class DynamicQueryStoreBase<TData>(TData initialValue) : IDisposable
{
    private readonly Dictionary<Guid, BehaviorSubject<TData>> _subjects = new();
    private readonly Dictionary<Guid, int> _refCounts = new();
    
    private readonly Lock _syncRoot = new();

    public IObservable<TData> Observe(Guid query)
    {
        if (query == Guid.Empty) return Observable.Empty<TData>();

        return Observable.Create<TData>(observer =>
        {
            BehaviorSubject<TData> subject;

            lock (_syncRoot)
            {
                if (!_refCounts.TryGetValue(query, out var count))
                {
                    count = 0;
                    _subjects[query] = new BehaviorSubject<TData>(initialValue);
                }
                
                _refCounts[query] = count + 1;
                subject = _subjects[query];
            }

            var subscription = subject.Subscribe(observer);

            return () =>
            {
                subscription.Dispose();
                ReleaseInternal(query);
            };
        });
    }

    private void ReleaseInternal(Guid query)
    {
        lock (_syncRoot)
        {
            if (!_refCounts.TryGetValue(query, out var count)) return;
            count--;
            
            if (count <= 0)
            {
                _refCounts.Remove(query);
                if (_subjects.Remove(query, out var subject))
                {
                    subject.Dispose();
                }
            }
            else
            {
                _refCounts[query] = count;
            }
        }
    }

    public void Emit(Guid query, TData data)
    {
        BehaviorSubject<TData>? subject = null;
        
        lock (_syncRoot)
        {
            if (_subjects.TryGetValue(query, out var s))
            {
                subject = s;
            }
        }

        subject?.OnNext(data);
    }

    public IEnumerable<Guid> GetActiveQueries()
    {
        lock (_syncRoot)
        {
            return _refCounts.Keys.ToList();
        }
    }

    public void Dispose()
    {
        lock (_syncRoot)
        {
            foreach (var subject in _subjects.Values)
            {
                subject.Dispose();
            }
            _subjects.Clear();
            _refCounts.Clear();
        }
        GC.SuppressFinalize(this);
    }
}