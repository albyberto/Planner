using System.Collections.Immutable;
using Planner.Model;

namespace Planner.Extensions;

public static class StatusModelExtensions
{
    private static readonly Dictionary<string, int> ExplicitOrder = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Backlog", 1 },
        { "To Do", 2 },
        { "Refinement", 3 },
        { "Refined", 4 },
        { "In Progress", 5 },
        { "Paused", 6 },
        { "Deployed Dev", 7 },
        { "Tested Dev", 8 },
        { "Customer Review", 9 },
        { "Done", 10 },
        { "Cancelled", 11 }
    };

    public static ImmutableArray<StatusModel> Sort(this IEnumerable<StatusModel> statuses)
    {
        var orderedStatuses = statuses
            .OrderBy(s => ExplicitOrder.TryGetValue(s.Name, out var order) ? order : int.MaxValue)
            .ThenBy(s => s.Name);

        return [.. orderedStatuses];
    }
}