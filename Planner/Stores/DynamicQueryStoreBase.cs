using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Planner.Stores;

/// <summary>
/// Base class for managing dynamic reactive state streams based on query keys (like JQL).
/// Automatically handles reference counting to clean up unused streams.
/// </summary>
/// <typeparam name="TData">The type of data emitted by the store.</typeparam>
public class DynamicQueryStoreBase<TData>(TData initialValue) : IDisposable
{
    private readonly ConcurrentDictionary<string, BehaviorSubject<TData>> _subjects = new();
    private readonly ConcurrentDictionary<string, int> _refCounts = new();

    /// <summary>
    /// Subscribes to a specific query stream. Creates it if it doesn't exist.
    /// </summary>
    public IObservable<TData> Observe(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return Observable.Empty<TData>();

        // Increment reference count for this query
        _refCounts.AddOrUpdate(query, 1, (_, count) => count + 1);

        // Get existing subject or create a new one with the initial value
        return _subjects.GetOrAdd(query, _ => new BehaviorSubject<TData>(initialValue))
                        .AsObservable();
    }

    /// <summary>
    /// Releases a subscription to a query. Cleans up memory if no one is listening anymore.
    /// </summary>
    public void Release(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return;

        if (_refCounts.TryGetValue(query, out int count))
        {
            if (count <= 1)
            {
                // No more subscribers: clean up completely
                _refCounts.TryRemove(query, out _);
                if (_subjects.TryRemove(query, out var subject))
                {
                    subject.Dispose();
                }
            }
            else
            {
                // Still other subscribers: just decrement the counter
                _refCounts[query] = count - 1;
            }
        }
    }

    /// <summary>
    /// Gets all currently active queries that have at least one subscriber.
    /// </summary>
    public IEnumerable<string> GetActiveQueries() => _refCounts.Keys;

    /// <summary>
    /// Emits new data to a specific query stream (called by the Background Service).
    /// </summary>
    public void Emit(string query, TData data)
    {
        if (_subjects.TryGetValue(query, out var subject))
        {
            subject.OnNext(data);
        }
    }

    public void Dispose()
    {
        foreach (var subject in _subjects.Values)
        {
            subject.Dispose();
        }
        _subjects.Clear();
        _refCounts.Clear();
        GC.SuppressFinalize(this);
    }
}