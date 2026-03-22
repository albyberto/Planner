using System.Collections.Immutable;
using System.Reactive;
using System.Reactive.Linq;
using Planner.Builders;
using Planner.Clients.Core;
using Planner.Mappers;
using Planner.Model;
using Planner.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace Planner.Background;

public class DashboardBackgroundService(
    FilterStore filterStore, 
    IssueStore issueStore, 
    IServiceScopeFactory scopeFactory, 
    ILogger<DashboardBackgroundService> logger) 
    : BackgroundServiceBase(10, logger) // 10 secondi passati al costruttore base
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var instantSubscription = filterStore.ObserveUpdates()
            .GroupBy(emit => emit.Key)
            .SelectMany(group => group
                .Throttle(TimeSpan.FromMilliseconds(500))
                .SelectMany(async emit =>
                {
                    await FetchAndEmitAsync(emit.Key, emit.Criteria, stoppingToken);
                    return Unit.Default;
                })
            )
            .Subscribe(
                _ => { },
                ex => logger.LogError(ex, "Errore nello stream istantaneo")
            );

        await base.ExecuteAsync(stoppingToken);
    }

    protected override async Task RunAsync(CancellationToken stoppingToken)
    {
        var activeFilters = filterStore.GetActiveFilters();
                
        foreach (var (key, criteria) in activeFilters)
        {
            if (stoppingToken.IsCancellationRequested) break;
            await FetchAndEmitAsync(key, criteria, stoppingToken);
        }
    }

    private async Task FetchAndEmitAsync(Guid key, SearchCriteria criteria, CancellationToken cancellationToken)
    {
        try
        {
            var jql = criteria.ToJql();

            if (string.IsNullOrWhiteSpace(jql))
            {
                issueStore.Emit(key, ImmutableArray<DashboardIssueModel>.Empty);
                return;
            }

            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IssueReadService>();

            var issues = await service.GetDashboardIssuesAsync(jql, cancellationToken);
            issueStore.Emit(key, [..issues.Select(issue => issue.ToDashboardModel())]);

            logger.LogInformation("Recuperate {Count} issue per la Key: {Key}", issues.Count, key);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            logger.LogError(ex, "Errore di comunicazione con Jira per la Key {Key}", key);
        }
    }
}