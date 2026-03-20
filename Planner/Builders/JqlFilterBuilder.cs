using JQLBuilder;
using JQLBuilder.Types.JqlTypes;
using Planner.Components.Shared.Filters.Model;
using Planner.Infrastructure.Domain.Enums;
using Planner.Model;

namespace Planner.Builders;

public static class JqlBuilderExtensions
{
    public static string ToJql(this IssuesSearchCriteria criteria)
    {
        if (string.IsNullOrWhiteSpace(criteria.Project?.Key)) return string.Empty;

        // 1. Costruiamo la base tipizzata con JQLBuilder
        var query = JqlBuilder.Query.Where(f => f.Project == criteria.Project.Key);

        // --- GESTIONE TIPI ---
        var includedTypes = criteria.Types.Included.Select(x => (JqlType)x.Name).ToArray();
        if (includedTypes.Length > 0) query = query.And(f => f.Issue.IssueType.In(includedTypes));

        var excludedTypes = criteria.Types.Excluded.Select(x => (JqlType)x.Name).ToArray();
        if (excludedTypes.Length > 0) query = query.And(f => f.Issue.IssueType.NotIn(excludedTypes));

        // --- GESTIONE STATUS ---
        var includedStatuses = criteria.Statuses.Included.Select(x => (JqlStatus)x.Name).ToArray();
        if (includedStatuses.Length > 0) query = query.And(f => f.Status.In(includedStatuses));

        var excludedStatuses = criteria.Statuses.Excluded.Select(x => (JqlStatus)x.Name).ToArray();
        if (excludedStatuses.Length > 0) query = query.And(f => f.Status.NotIn(excludedStatuses));

        // --- GESTIONE COMPONENTI ---
        var includedComponents = criteria.Components.Included.Select(x => (JqlComponent)x.Name).ToArray();
        if (includedComponents.Length > 0) query = query.And(f => f.Component.In(includedComponents));

        var excludedComponents = criteria.Components.Excluded.Select(x => (JqlComponent)x.Name).ToArray();
        if (excludedComponents.Length > 0) query = query.And(f => f.Component.NotIn(excludedComponents));

        // --- GESTIONE ETICHETTE ---
        var includedLabels = criteria.Labels.Included.Select(x => (JqlLabels)x.Name).ToArray();
        if (includedLabels.Length > 0) query = query.And(f => f.Labels.In(includedLabels));

        var excludedLabels = criteria.Labels.Excluded.Select(x => (JqlLabels)x.Name).ToArray();
        if (excludedLabels.Length > 0) query = query.And(f => f.Labels.NotIn(excludedLabels));

        // --- GESTIONE ASSIGNEES E UNASSIGNED ---
        var includedAssignees = criteria.Assignees.Included.Select(x => (JqlHistoricalJqlUser)x.EmailAddress).ToArray();
        var excludedAssignees = criteria.Assignees.Excluded.Select(x => (JqlHistoricalJqlUser)x.EmailAddress).ToArray();

        if (includedAssignees.Length > 0 && criteria.IncludeUnassigned)
            query = query.And(f => f.User.Assignee.In(includedAssignees) | f.User.Assignee.Is(v => v.Empty));
        else if (includedAssignees.Length > 0)
            query = query.And(f => f.User.Assignee.In(includedAssignees));
        else if (criteria.IncludeUnassigned) query = query.And(f => f.User.Assignee.Is(v => v.Empty));

        if (excludedAssignees.Length > 0) query = query.And(f => f.User.Assignee.NotIn(excludedAssignees));

        if (excludedAssignees.Length > 0) query = query.And(f => f.User.Assignee.NotIn(excludedAssignees));

        // --- GESTIONE FILTRI DATA ---
        foreach (var (field, preset) in criteria.DateFilters)
        {
            var range = GetDateRange(preset);
            if (!range.HasValue) continue;

            var start = DateOnly.FromDateTime(range.Value.Start);
            var end = DateOnly.FromDateTime(range.Value.End);

            query = query.And(f => (f.Date.Only[field] >= start) & (f.Date.Only[field] <= end));
        }

        if (criteria.StatusTransition.Preset != Preset.None || criteria.StatusTransition.Item is not null)
        {
            var filter = criteria.StatusTransition;
            var range = GetDateRange(filter.Preset);
        
            var statusName = filter.Item?.Name; 

            if (range.HasValue && statusName != null)
            {
                var start = range.Value.Start;
                var end = range.Value.End;
                query = query.And(f => f.Status.Changed(c => c.To(statusName).During(start, end)));
            }
            else if (statusName != null)
            {
                query = query.And(f => f.Status.Changed(c => c.To(statusName)));
            }
            else if (range.HasValue)
            {
                var start = range.Value.Start;
                var end = range.Value.End;
                query = query.And(f => f.Status.Changed(c => c.During(start, end)));
            }
        }

        return query.ToString();
    }

    private static (DateTime Start, DateTime End)? GetDateRange(Preset preset)
    {
        if (preset == Preset.None) return null;

        var today = DateTime.UtcNow.Date;
        var firstDayOfThisMonth = new DateTime(today.Year, today.Month, 1);

        return preset switch
        {
            Preset.Today => (today, today),
            Preset.Yesterday => (today.AddDays(-1), today.AddDays(-1)),
            Preset.Tomorrow => (today.AddDays(1), today.AddDays(1)),

            Preset.ThisWeek => (StartOfWeek(today), StartOfWeek(today).AddDays(6)),
            Preset.LastWeek => (StartOfWeek(today).AddDays(-7), StartOfWeek(today).AddDays(-1)),
            Preset.NextWeek => (StartOfWeek(today).AddDays(7), StartOfWeek(today).AddDays(13)),

            // Dal primo all'ultimo giorno del mese precedente
            Preset.LastMonth => (firstDayOfThisMonth.AddMonths(-1), firstDayOfThisMonth.AddDays(-1)),

            // Dal primo all'ultimo giorno del mese successivo
            Preset.NextMonth => (firstDayOfThisMonth.AddMonths(1), firstDayOfThisMonth.AddMonths(2).AddDays(-1)),

            Preset.Last7Days => (today.AddDays(-7), today),
            Preset.Next7Days => (today, today.AddDays(7)),

            _ => null
        };
    }

    private static DateTime StartOfWeek(DateTime date)
    {
        const DayOfWeek firstDay = DayOfWeek.Monday;
        var diff = (7 + (date.DayOfWeek - firstDay)) % 7;
        return date.AddDays(-diff).Date;
    }
}