using System.Collections.Immutable;

namespace Planner.Components.Shared.Filters.Model;

public record FilterSelection<T>(ImmutableHashSet<T> Included, ImmutableHashSet<T> Excluded) : IFilterSelection<T>
{
    public static FilterSelection<T> Empty => new(ImmutableHashSet<T>.Empty, ImmutableHashSet<T>.Empty);

    public IFilterSelection<T> Toggle(T value)
    {
        if (Included.Contains(value))
            return new FilterSelection<T>(Included.Remove(value), Excluded.Add(value));

        if (Excluded.Contains(value))
            return new FilterSelection<T>(Included.Add(value), Excluded.Remove(value));

        return this;
    }

    public IFilterSelection<T> Remove(T value) => new FilterSelection<T>(Included.Remove(value), Excluded.Remove(value));

    public IFilterSelection<T> Add(T value) => Included.Contains(value) || Excluded.Contains(value) ? this : this with { Included = Included.Add(value) };
}