using System.Collections.Immutable;

namespace Planner.Components.Shared.Filters.Model;

public interface IFilterSelection<T>
{
    ImmutableHashSet<T> Included { get; }
    ImmutableHashSet<T> Excluded { get; }
    ImmutableHashSet<T> All => Included.Concat(Excluded).ToImmutableHashSet();

    IFilterSelection<T> Toggle(T value);
    IFilterSelection<T> Remove(T value);
    IFilterSelection<T> Add(T value);
}