using System.Collections.Immutable;

namespace Planner.Components.Shared.Filters.Model;

public interface IFilterSelection<T>
{
    ImmutableHashSet<T> Included { get; }
    ImmutableHashSet<T> Excluded { get; }
    ImmutableHashSet<(T Item, bool Included)> Merged => Included.Select(item => (item, true)).Concat(Excluded.Select(item => (item, false))).ToImmutableHashSet();
    ImmutableHashSet<T> Items => Included.Select(item => item).Concat(Excluded.Select(item => item)).ToImmutableHashSet();

    IFilterSelection<T> Toggle(T value);
    IFilterSelection<T> Remove(T value);
    IFilterSelection<T> Add(T value);
}