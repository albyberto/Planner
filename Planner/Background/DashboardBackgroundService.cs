using System.Collections.Immutable;
using System.Reactive;
using System.Reactive.Linq;
using Planner.Builders;
using Planner.Infrastructure.Clients;
using Planner.Model;
using Planner.Stores;

namespace Planner.Background;

public class DashboardBackgroundService(FilterStore filterStore, IssueStore issueStore, JiraReadClient readClient, ILogger<DashboardBackgroundService> logger) : BackgroundServiceBase(1, logger)
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var instantSubscription = filterStore.ObserveGlobal()
            .GroupBy(emit => emit.Key)
            .SelectMany(group => group
                .Throttle(TimeSpan.FromMilliseconds(500))
                .SelectMany(async emit =>
                {
                    await FetchAndEmitAsync(emit, stoppingToken);
                    return Unit.Default;
                })
            )
            .Subscribe(
                _ => { },
                ex => logger.LogError(ex, "Errore nello stream istantaneo")
            );

        try
        {
            await base.ExecuteAsync(stoppingToken);
        }
        finally
        {
            instantSubscription.Dispose();
        }
    }

    protected override async Task RunAsync(CancellationToken stoppingToken)
    {
        var activeTabs = filterStore.GetFiltersForPolling(TimeSpan.FromSeconds(60));

        foreach (var emit in activeTabs)
        {
            if (stoppingToken.IsCancellationRequested) break;

            await FetchAndEmitAsync(emit, stoppingToken);
        }
    }

    private async Task FetchAndEmitAsync(Emit<IssueSearchCriteria> emit, CancellationToken cancellationToken)
    {
        try
        {
            var jql = emit.Value.ToJql();

            if (string.IsNullOrWhiteSpace(jql))
            {
                issueStore.Emit(emit.Key, []);
                return;
            }

            ImmutableHashSet<string> fields = ["summary", "status", "assignee", "fixVersions", "issuetype", "components", "labels", "project"];
            var issues = await readClient.GetIssuesAsync(jql, fields, cancellationToken: cancellationToken);

            filterStore.MarkAsFetched(emit.Key);

            issueStore.Emit(emit.Key, [..issues.Select(issue => new IssueModel(issue, () => readClient.GetAvailableTransitionsAsync(issue.Key, CancellationToken.None)))]);

            logger.LogInformation("Recuperate {Count} issue per la Key: {Key}", issues.Count, emit.Key);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Errore di comunicazione con Jira per la Key {Key}", emit.Key);
        }
    }
}