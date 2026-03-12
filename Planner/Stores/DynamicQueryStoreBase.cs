using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Planner.Stores;

public class DynamicQueryStoreBase<TData>(TData initialValue) : IDisposable
{
    // Usiamo un lock esplicito e dizionari standard. 
    // Mantenere sincronizzati due ConcurrentDictionary senza lock è quasi impossibile senza incorrere in race conditions.
    private readonly Dictionary<string, BehaviorSubject<TData>> _subjects = new();
    private readonly Dictionary<string, int> _refCounts = new();
    private readonly object _syncRoot = new();

    public IObservable<TData> Observe(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return Observable.Empty<TData>();

        // Observable.Create ci permette di iniettare logica quando qualcuno fa .Subscribe() e .Dispose()
        return Observable.Create<TData>(observer =>
        {
            BehaviorSubject<TData> subject;

            // 1. Inizio Sottoscrizione: Incrementiamo in modo sicuro
            lock (_syncRoot)
            {
                if (!_refCounts.TryGetValue(query, out var count))
                {
                    // Primo iscritto: creiamo il subject
                    count = 0;
                    _subjects[query] = new BehaviorSubject<TData>(initialValue);
                }
                
                _refCounts[query] = count + 1;
                subject = _subjects[query];
            }

            // 2. Passiamo i dati al chiamante
            var subscription = subject.Subscribe(observer);

            // 3. Fine Sottoscrizione (Return Action): Eseguita AUTOMATICAMENTE quando la UI fa .Dispose()
            return () =>
            {
                subscription.Dispose();
                ReleaseInternal(query);
            };
        });
    }

    private void ReleaseInternal(string query)
    {
        lock (_syncRoot)
        {
            if (_refCounts.TryGetValue(query, out var count))
            {
                count--;
                if (count <= 0)
                {
                    // Nessun iscritto rimasto: facciamo pulizia completa
                    _refCounts.Remove(query);
                    if (_subjects.TryGetValue(query, out var subject))
                    {
                        _subjects.Remove(query);
                        subject.Dispose();
                    }
                }
                else
                {
                    // Ci sono ancora iscritti, aggiorniamo solo il counter
                    _refCounts[query] = count;
                }
            }
        }
    }

    public void Emit(string query, TData data)
    {
        BehaviorSubject<TData>? subject = null;
        
        // Prendiamo il subject in modo sicuro
        lock (_syncRoot)
        {
            if (_subjects.TryGetValue(query, out var s))
            {
                subject = s;
            }
        }

        // Emettiamo fuori dal lock per evitare potenziali deadlocks se gli observer sono lenti
        subject?.OnNext(data);
    }

    public IEnumerable<string> GetActiveQueries()
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