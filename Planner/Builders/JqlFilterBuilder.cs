using System.Globalization;
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
        return "project = OPNMS";
    }
}

//         if (string.IsNullOrWhiteSpace(criteria.ProjectKey)) throw new ArgumentException("ProjectKey is required to build JQL query.");
//
//         // Splittiamo i filtri In e NotIn estraendo solo il valore stringa
//         var inTypes = criteria.Types.Where(x => !x.IsExcluded).Select(x => x.Value).ToArray();
//         var notInTypes = criteria.Types.Where(x => x.IsExcluded).Select(x => x.Value).ToArray();
//         
//         var inStatuses = criteria.Statuses.Where(x => !x.IsExcluded).Select(x => x.Value).ToArray();
//         var notInStatuses = criteria.Statuses.Where(x => x.IsExcluded).Select(x => x.Value).ToArray();
//
//         var inComponents = criteria.Components.Where(x => !x.IsExcluded).Select(x => x.Value).ToArray();
//         var notInComponents = criteria.Components.Where(x => x.IsExcluded).Select(x => x.Value).ToArray();
//
//         var inLabels = criteria.Labels.Where(x => !x.IsExcluded).Select(x => x.Value).ToArray();
//         var notInLabels = criteria.Labels.Where(x => x.IsExcluded).Select(x => x.Value).ToArray();
//
//         // 1. Catena Fluida (Inclusi ed Esclusi per ogni tipologia)
//         var query = JqlBuilder.Query.Where(f => f.Project == criteria.ProjectKey)
//             
//             .AndIf(inTypes.Length > 0, f => f.Issue.IssueType.In(inTypes.Select(x => (JqlType)x).ToArray()))
//             .AndIf(notInTypes.Length > 0, f => f.Issue.IssueType.NotIn(notInTypes.Select(x => (JqlType)x).ToArray()))
//             
//             .AndIf(inStatuses.Length > 0, f => f.Status.In(inStatuses.Select(x => (JqlStatus)x).ToArray()))
//             .AndIf(notInStatuses.Length > 0, f => f.Status.NotIn(notInStatuses.Select(x => (JqlStatus)x).ToArray()))
//             
//             .AndIf(inComponents.Length > 0, f => f.Component.In(inComponents.Select(x => (JqlComponent)x).ToArray()))
//             .AndIf(notInComponents.Length > 0, f => f.Component.NotIn(notInComponents.Select(x => (JqlComponent)x).ToArray()))
//             
//             .AndIf(inLabels.Length > 0, f => f.Labels.In(inLabels.Select(x => (JqlLabels)x).ToArray()))
//             .AndIf(notInLabels.Length > 0, f => f.Labels.NotIn(notInLabels.Select(x => (JqlLabels)x).ToArray()));
//
//         // 2. Pattern Matching Switch per Assignees (Supporta In e NotIn dinamicamente)
//         var inAssignees = criteria.Assignees.Where(x => !x.IsExcluded).Select(x => x.Value).ToArray();
//         var notInAssignees = criteria.Assignees.Where(x => x.IsExcluded).Select(x => x.Value).ToArray();
//         
//         var hasInAssignees = inAssignees.Length > 0;
//
//         // Se l'utente ha escluso specifici Assignees, li mettiamo a prescindere dal resto
//         query = query.AndIf(notInAssignees.Length > 0, f => f.User.Assignee.NotIn(notInAssignees.Select(x => (JqlHistoricalJqlUser)x).ToArray()));
//
//         // Logica per gli Assignees Inclusi vs Unassigned
//         query = (hasInAssignees, criteria.IncludeUnassigned) switch
//         {
//             (true, true) => query.And(f => f.User.Assignee.In(inAssignees.Select(x => (JqlHistoricalJqlUser)x).ToArray()) | f.User.Assignee.Is(v => v.Empty)),
//             (true, false) => query.And(f => f.User.Assignee.In(inAssignees.Select(x => (JqlHistoricalJqlUser)x).ToArray())),
//             (false, true) => query.And(f => f.User.Assignee.Is(v => v.Empty)),
//             (false, false) => query
//         };
//
//         var jql = query.ToString();
//
//         if (criteria.TimeFilter is { } timeFilter)
//         {
//             var timeCondition = BuildTimeCondition(timeFilter);
//             if (!string.IsNullOrWhiteSpace(timeCondition))
//             {
//                 jql = string.IsNullOrWhiteSpace(jql)
//                     ? timeCondition
//                     : $"{jql} AND {timeCondition}";
//             }
//         }
//
//         return jql;
//     }
//
//     private static JqlFilter AndIf(this JqlFilter query, bool condition, Func<Fields, Bool> filter) => condition ? query.And(filter) : query;
//
//     private static string BuildTimeCondition(TimeFilter timeFilter)
//     {
//         if (timeFilter.Preset == RelativeTimePreset.None)
//         {
//             return string.Empty;
//         }
//
//         return timeFilter.Target switch
//         {
//             TimeFilterTarget.CreatedDate => BuildCreatedCondition(timeFilter),
//             TimeFilterTarget.ResolvedDate => BuildResolvedCondition(timeFilter),
//             TimeFilterTarget.StatusTransitionDate => BuildStatusTransitionCondition(timeFilter),
//             _ => string.Empty
//         };
//     }
//
//     private static string BuildCreatedCondition(TimeFilter filter) =>
//         BuildDateFieldCondition("created", filter);
//
//     private static string BuildResolvedCondition(TimeFilter filter) =>
//         // Prefer resolutiondate when available; adjust here if your Jira uses a different field.
//         BuildDateFieldCondition("resolutiondate", filter);
//
//     private static string BuildDateFieldCondition(string fieldName, TimeFilter filter)
//     {
//         if (filter.Preset == RelativeTimePreset.CustomRange)
//         {
//             var (from, to) = (filter.Range.From, filter.Range.To);
//             if (from is null && to is null) return string.Empty;
//
//             var parts = new List<string>();
//             if (from is not null)
//             {
//                 parts.Add($"{fieldName} >= \"{ToJiraDate(from.Value)}\"");
//             }
//
//             if (to is not null)
//             {
//                 parts.Add($"{fieldName} <= \"{ToJiraDate(to.Value)}\"");
//             }
//
//             return string.Join(" AND ", parts);
//         }
//
//         var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
//         DateOnly? fromPreset = filter.Preset switch
//         {
//             RelativeTimePreset.Last7Days => today.AddDays(-7),
//             RelativeTimePreset.Last30Days => today.AddDays(-30),
//             RelativeTimePreset.ThisWeek => StartOfWeek(today),
//             RelativeTimePreset.LastWeek => StartOfWeek(today).AddDays(-7),
//             RelativeTimePreset.ThisMonth => new DateOnly(today.Year, today.Month, 1),
//             RelativeTimePreset.LastMonth => new DateOnly(today.AddMonths(-1).Year, today.AddMonths(-1).Month, 1),
//             _ => null
//         };
//
//         if (fromPreset is null)
//         {
//             return string.Empty;
//         }
//
//         return $"{fieldName} >= \"{ToJiraDate(fromPreset.Value)}\"";
//     }
//
//     private static string BuildStatusTransitionCondition(TimeFilter filter)
//     {
//         if (string.IsNullOrWhiteSpace(filter.StatusName))
//         {
//             return string.Empty;
//         }
//
//         DateOnly? from;
//         DateOnly? to;
//
//         if (filter.Preset == RelativeTimePreset.CustomRange)
//         {
//             from = filter.Range.From;
//             to = filter.Range.To;
//         }
//         else
//         {
//             var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
//             from = filter.Preset switch
//             {
//                 RelativeTimePreset.Last7Days => today.AddDays(-7),
//                 RelativeTimePreset.Last30Days => today.AddDays(-30),
//                 RelativeTimePreset.ThisWeek => StartOfWeek(today),
//                 RelativeTimePreset.LastWeek => StartOfWeek(today).AddDays(-7),
//                 RelativeTimePreset.ThisMonth => new DateOnly(today.Year, today.Month, 1),
//                 RelativeTimePreset.LastMonth => new DateOnly(today.AddMonths(-1).Year, today.AddMonths(-1).Month, 1),
//                 _ => null
//             };
//             to = null;
//         }
//
//         if (from is null && to is null)
//         {
//             return string.Empty;
//         }
//
//         var fromLiteral = from is null ? null : ToJiraDate(from.Value);
//         var toLiteral = to is null ? null : ToJiraDate(to.Value);
//
//         if (fromLiteral is not null && toLiteral is not null)
//         {
//             return $"status CHANGED TO \"{filter.StatusName}\" DURING (\"{fromLiteral}\", \"{toLiteral}\")";
//         }
//
//         if (fromLiteral is not null)
//         {
//             return $"status CHANGED TO \"{filter.StatusName}\" AFTER \"{fromLiteral}\"";
//         }
//
//         // only "to" set
//         return $"status CHANGED TO \"{filter.StatusName}\" BEFORE \"{toLiteral}\"";
//     }
//
//     private static string ToJiraDate(DateOnly date) =>
//         date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
//
//     private static DateOnly StartOfWeek(DateOnly date)
//     {
//         const DayOfWeek firstDay = DayOfWeek.Monday;
//         int diff = (7 + (date.DayOfWeek - firstDay)) % 7;
//         return date.AddDays(-diff);
//     }
// }