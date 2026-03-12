using JQLBuilder;
using JQLBuilder.Infrastructure.Operators;
using JQLBuilder.Types.JqlTypes;
using Planner.Model;
using JqlFilter = JQLBuilder.Render.JqlFilter;

namespace Planner.Builders;

public static class JqlBuilderExtensions
{
    public static string ToJqlBuilder(this IssueSearchCriteria criteria)
    {
        if (string.IsNullOrWhiteSpace(criteria.ProjectKey)) throw new ArgumentException("ProjectKey is required to build JQL query.");

        // 1. Catena Fluida per tutti i filtri diretti
        var query = JqlBuilder.Query.Where(f => f.Project == criteria.ProjectKey)
            .AndIf(criteria.Types.Count > 0, f => f.Issue.IssueType.In(criteria.Types.ToJqlArray(x => (JqlType)x)))
            .AndIf(criteria.Statuses.Count > 0, f => f.Status.In(criteria.Statuses.ToJqlArray(x => (JqlStatus)x)))
            .AndIf(criteria.Components.Count > 0, f => f.Component.In(criteria.Components.ToJqlArray(x => (JqlComponent)x)))
            .AndIf(criteria.Labels.Count > 0, f => f.Labels.In(criteria.Labels.ToJqlArray(x => (JqlLabels)x)));

        // 2. Pattern Matching Switch per la logica combinata degli Assignees
        var hasAssignees = criteria.Assignees.Count > 0;

        query = (hasAssignees, criteria.IncludeUnassigned) switch
        {
            // Se ci sono assignee E vogliamo includere i non assegnati
            (true, true) => query.And(f =>
                f.User.Assignee.In(criteria.Assignees.ToJqlArray(x => (JqlHistoricalJqlUser)x)) | f.User.Assignee.Is(v => v.Empty)),

            // Se ci sono assignee MA NON vogliamo includere i non assegnati
            (true, false) => query.And(f =>
                f.User.Assignee.In(criteria.Assignees.ToJqlArray(x => (JqlHistoricalJqlUser)x))),

            // Se NON ci sono assignee MA vogliamo includere i non assegnati
            (false, true) => query.And(f =>
                f.User.Assignee.Is(v => v.Empty)),

            // Se NON ci sono assignee e NON vogliamo i non assegnati (nessuna azione)
            (false, false) => query
        };

        return query.ToString();
    }

    private static JqlFilter AndIf(this JqlFilter query, bool condition, Func<Fields, Bool> filter) =>
        condition ? query.And(filter) : query;

    private static T[] ToJqlArray<T>(this IEnumerable<string> source, Func<string, T> selector) =>
        source.Select(selector).ToArray();
}