using System.Globalization;
using JQLBuilder;
using JQLBuilder.Infrastructure.Operators;
using JQLBuilder.Types.JqlTypes;
using Planner.Components.Shared.Filters.Model;
using Planner.Model;

namespace Planner.Builders;

public static class JqlBuilderExtensions
{
    public static string ToJql(this IssuesSearchCriteria criteria)
    {
        if (string.IsNullOrWhiteSpace(criteria.Project?.Key))
        {
            return string.Empty; // Nessun progetto, nessuna query
        }

        // 1. Costruiamo la base tipizzata con JQLBuilder
        var query = JqlBuilder.Query.Where(f => f.Project == criteria.Project.Key);

        // Aggiungiamo i filtri in modo sicuro e senza problemi di tipo
        var types = criteria.Types.Items.Select(x => (JqlType)x.Name).ToArray();
        if (types.Length > 0)
        {
            query = query.And(f => f.Issue.IssueType.In(types));
        }

        var statuses = criteria.Statuses.Items.Select(x => (JqlStatus)x.Name).ToArray();
        if (statuses.Length > 0)
        {
            query = query.And(f => f.Status.In(statuses));
        }

        var components = criteria.Components.Items.Select(x => (JqlComponent)x.Name).ToArray();
        if (components.Length > 0)
        {
            query = query.And(f => f.Component.In(components));
        }

        var labels = criteria.Labels.Items.Select(x => (JqlLabels)x.Name).ToArray();
        if (labels.Length > 0)
        {
            query = query.And(f => f.Labels.In(labels));
        }

        // Estraiamo la stringa base JQL
        var jql = query.ToString();
        var additionalConditions = new List<string>();

        // // 2. Processiamo i filtri Data Dinamici (DateFilters)
        // foreach (var (field, preset) in criteria.DateFilters)
        // {
        //     var dateLiteral = GetJiraDateFromPreset(preset);
        //     if (!string.IsNullOrEmpty(dateLiteral))
        //     {
        //         // Es: created >= "2023-10-25"
        //         additionalConditions.Add($"{field} >= \"{dateLiteral}\"");
        //     }
        // }

        // 3. Processiamo i filtri di Transizione Dinamici (TransitionFilters)
        // foreach (var (field, filter) in criteria.TransitionFilters)
        // {
        //     var transitionCondition = BuildTransitionCondition(field, filter);
        //     if (!string.IsNullOrEmpty(transitionCondition))
        //     {
        //         // Es: status CHANGED TO "Done" AFTER "2023-10-25"
        //         additionalConditions.Add(transitionCondition);
        //     }
        // }

        // 4. Uniamo le query dinamiche alla query base
        if (additionalConditions.Any())
        {
            var extraJql = string.Join(" AND ", additionalConditions);
            jql = string.IsNullOrWhiteSpace(jql) ? extraJql : $"{jql} AND {extraJql}";
        }

        return jql;
    }

    // --- Metodi di Supporto ---

    // private static string BuildTransitionCondition(string field, TransitionFilter<string> filter)
    // {
    //     var parts = new List<string> { $"{field} CHANGED" };
    //
    //     if (!string.IsNullOrWhiteSpace(filter.Item))
    //     {
    //         parts.Add($"TO \"{filter.Item}\"");
    //     }
    //
    //     if (filter.Preset != DatePreset.None)
    //     {
    //         var dateLiteral = GetJiraDateFromPreset(filter.Preset);
    //         if (dateLiteral != null)
    //         {
    //             parts.Add($"AFTER \"{dateLiteral}\"");
    //         }
    //     }
    //
    //     // Se l'utente non ha impostato né l'item né il preset, non generiamo la query di transizione
    //     if (parts.Count == 1) return string.Empty;
    //
    //     return string.Join(" ", parts);
    // }
    //
    // private static string? GetJiraDateFromPreset(DatePreset preset)
    // {
    //     if (preset == DatePreset.None) return null;
    //
    //     var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
    //     DateOnly? date = preset switch
    //     {
    //         DatePreset.Today => today,
    //         DatePreset.Yesterday => today.AddDays(-1),
    //         DatePreset.LastWeek => StartOfWeek(today).AddDays(-7),
    //         DatePreset.LastMonth => new DateOnly(today.AddMonths(-1).Year, today.AddMonths(-1).Month, 1),
    //         _ => null
    //     };
    //
    //     return date?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    // }

    private static DateOnly StartOfWeek(DateOnly date)
    {
        const DayOfWeek firstDay = DayOfWeek.Monday;
        int diff = (7 + (date.DayOfWeek - firstDay)) % 7;
        return date.AddDays(-diff);
    }
}