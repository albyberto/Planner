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

    public void Emit(Guid key, ImmutableArray<DashboardIssueModel> incoming)
    {
        var subject = GetOrCreate(key);
        var current = subject.Value;

        // First emission or empty current → emit directly
        if (current.IsEmpty)
        {
            subject.OnNext(incoming);
            return;
        }

        var incomingById = new Dictionary<string, DashboardIssueModel>(incoming.Length);
        foreach (var issue in incoming)
            incomingById[issue.Id] = issue;

        var changed = false;
        var merged = new List<DashboardIssueModel>(current.Length);

        // Walk current list preserving order, update in-place if changed
        foreach (var existing in current)
        {
            if (incomingById.Remove(existing.Id, out var fresh))
            {
                if (!existing.Equals(fresh))
                {
                    UpdateInPlace(existing, fresh);
                    changed = true;
                }
                merged.Add(existing); // keep same reference
            }
            else
            {
                changed = true; // issue removed
            }
        }

        // Append truly new issues
        foreach (var newIssue in incomingById.Values)
        {
            merged.Add(newIssue);
            changed = true;
        }

        if (changed)
        {
            subject.OnNext([..merged]);
        }
    }

    public void Unregister(Guid key)
    {
        if (!_store.TryRemove(key, out var subject)) return;
        
        subject.OnCompleted();
        subject.Dispose();
    }

    private BehaviorSubject<ImmutableArray<DashboardIssueModel>> GetOrCreate(Guid key) => _store.GetOrAdd(key, static (_, initVal) => new(initVal), _initialValue);

    private static void UpdateInPlace(DashboardIssueModel target, DashboardIssueModel source)
    {
        target.Key = source.Key;
        target.Self = source.Self;
        target.ProjectKey = source.ProjectKey;
        target.Summary = source.Summary;
        target.Status = source.Status;
        target.Assignee = source.Assignee;
        target.Type = source.Type;
        target.Components = source.Components;
        target.Labels = source.Labels;
        target.FixVersions = source.FixVersions;
        target.Transitions = source.Transitions;
        target.StartDate = source.StartDate;
        target.EndDate = source.EndDate;
        target.DueDate = source.DueDate;
        target.Stats = source.Stats;
    }
}