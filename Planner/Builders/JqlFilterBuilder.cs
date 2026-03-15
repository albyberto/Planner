using JQLBuilder;
using JQLBuilder.Infrastructure.Operators;
using JQLBuilder.Types.JqlTypes;
using Planner.Model;
using JqlFilter = JQLBuilder.Render.JqlFilter;

namespace Planner.Builders;

public static class JqlBuilderExtensions
{
    public static string ToJql(this IssuesSearchCriteria criteria)
    {
        if (string.IsNullOrWhiteSpace(criteria.ProjectKey)) throw new ArgumentException("ProjectKey is required to build JQL query.");

        // Splittiamo i filtri In e NotIn estraendo solo il valore stringa
        var inTypes = criteria.Types.Where(x => !x.IsExcluded).Select(x => x.Value).ToArray();
        var notInTypes = criteria.Types.Where(x => x.IsExcluded).Select(x => x.Value).ToArray();
        
        var inStatuses = criteria.Statuses.Where(x => !x.IsExcluded).Select(x => x.Value).ToArray();
        var notInStatuses = criteria.Statuses.Where(x => x.IsExcluded).Select(x => x.Value).ToArray();

        var inComponents = criteria.Components.Where(x => !x.IsExcluded).Select(x => x.Value).ToArray();
        var notInComponents = criteria.Components.Where(x => x.IsExcluded).Select(x => x.Value).ToArray();

        var inLabels = criteria.Labels.Where(x => !x.IsExcluded).Select(x => x.Value).ToArray();
        var notInLabels = criteria.Labels.Where(x => x.IsExcluded).Select(x => x.Value).ToArray();

        // 1. Catena Fluida (Inclusi ed Esclusi per ogni tipologia)
        var query = JqlBuilder.Query.Where(f => f.Project == criteria.ProjectKey)
            
            .AndIf(inTypes.Length > 0, f => f.Issue.IssueType.In(inTypes.Select(x => (JqlType)x).ToArray()))
            .AndIf(notInTypes.Length > 0, f => f.Issue.IssueType.NotIn(notInTypes.Select(x => (JqlType)x).ToArray()))
            
            .AndIf(inStatuses.Length > 0, f => f.Status.In(inStatuses.Select(x => (JqlStatus)x).ToArray()))
            .AndIf(notInStatuses.Length > 0, f => f.Status.NotIn(notInStatuses.Select(x => (JqlStatus)x).ToArray()))
            
            .AndIf(inComponents.Length > 0, f => f.Component.In(inComponents.Select(x => (JqlComponent)x).ToArray()))
            .AndIf(notInComponents.Length > 0, f => f.Component.NotIn(notInComponents.Select(x => (JqlComponent)x).ToArray()))
            
            .AndIf(inLabels.Length > 0, f => f.Labels.In(inLabels.Select(x => (JqlLabels)x).ToArray()))
            .AndIf(notInLabels.Length > 0, f => f.Labels.NotIn(notInLabels.Select(x => (JqlLabels)x).ToArray()));

        // 2. Pattern Matching Switch per Assignees (Supporta In e NotIn dinamicamente)
        var inAssignees = criteria.Assignees.Where(x => !x.IsExcluded).Select(x => x.Value).ToArray();
        var notInAssignees = criteria.Assignees.Where(x => x.IsExcluded).Select(x => x.Value).ToArray();
        
        var hasInAssignees = inAssignees.Length > 0;

        // Se l'utente ha escluso specifici Assignees, li mettiamo a prescindere dal resto
        query = query.AndIf(notInAssignees.Length > 0, f => f.User.Assignee.NotIn(notInAssignees.Select(x => (JqlHistoricalJqlUser)x).ToArray()));

        // Logica per gli Assignees Inclusi vs Unassigned
        query = (hasInAssignees, criteria.IncludeUnassigned) switch
        {
            (true, true) => query.And(f => f.User.Assignee.In(inAssignees.Select(x => (JqlHistoricalJqlUser)x).ToArray()) | f.User.Assignee.Is(v => v.Empty)),
            (true, false) => query.And(f => f.User.Assignee.In(inAssignees.Select(x => (JqlHistoricalJqlUser)x).ToArray())),
            (false, true) => query.And(f => f.User.Assignee.Is(v => v.Empty)),
            (false, false) => query
        };

        return query.ToString();
    }

    private static JqlFilter AndIf(this JqlFilter query, bool condition, Func<Fields, Bool> filter) => condition ? query.And(filter) : query;
}