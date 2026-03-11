using System.Collections.Immutable;
using Planner.Domain;
using Planner.Infrastructure.Clients;
using Planner.Model;
using Planner.Services;
using Planner.Stores;

namespace Planner.Background;

/// <summary>
/// Background service that subscribes to the FilterStore.
/// When a filter is emitted by a page, this service resolves the JQL,
/// fetches issues from Jira, and emits the results to the IssueStore.
/// </summary>
public class DashboardBackgroundService(
    FilterStore filterStore,
    IssueStore issueStore,
    JiraReadClient readClient,
    JqlFilterBuilder queryBuilder,
    ILogger<DashboardBackgroundService> logger) : BackgroundService
{
    /// <summary>
    /// Tracks the last filter processed per page to avoid redundant API calls.
    /// </summary>
    private readonly Dictionary<string, FilterModel> _lastProcessed = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait briefly to let the application start up
        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

        logger.LogInformation("DashboardBackgroundService started.");

        // Simple polling loop: checks active queries every 500ms
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var activeQueries = filterStore.GetActiveQueries().ToList();

                foreach (var pageKey in activeQueries)
                {
                    // Take the current value from the BehaviorSubject
                    FilterModel? currentFilter = null;

                    using var subscription = filterStore.Observe(pageKey)
                        .Subscribe(f => currentFilter = f);

                    // Release the subscription we just opened for peeking
                    filterStore.Release(pageKey);

                    if (currentFilter is null || string.IsNullOrWhiteSpace(currentFilter.ProjectKey))
                        continue;

                    // Skip if this exact filter was already processed
                    if (_lastProcessed.TryGetValue(pageKey, out var last) && last == currentFilter)
                        continue;

                    _lastProcessed[pageKey] = currentFilter;

                    await ProcessFilterAsync(pageKey, currentFilter, stoppingToken);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Error in DashboardBackgroundService loop.");
            }

            await Task.Delay(500, stoppingToken);
        }

        logger.LogInformation("DashboardBackgroundService stopped.");
    }

    private async Task ProcessFilterAsync(string pageKey, FilterModel filterModel, CancellationToken cancellationToken)
    {
        var jql = queryBuilder.BuildDashboardQuery(filterModel);

        logger.LogInformation("Fetching issues for page {PageKey} with JQL: {Jql}", pageKey, jql);

        var issues = await readClient.GetIssuesAsync(jql, cancellationToken: cancellationToken);

        var models = issues
            .Select(issue => new IssueModel(issue, () => Task.FromResult<IReadOnlyList<Transition>>([])))
            .ToImmutableList();

        issueStore.Emit(pageKey, models);

        logger.LogInformation("Emitted {Count} issues for page {PageKey}", models.Count, pageKey);
    }
}