using Planner.Clients;
using Planner.Services;

namespace Planner.Background;

/// <summary>
/// Background service che esegue il polling periodico di Jira per le issue del Dashboard.
/// Notifica i componenti Blazor tramite <see cref="JiraNotificationService"/>.
/// </summary>
public class JiraDashboardBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly JiraNotificationService _notificationService;
    private readonly ILogger<JiraDashboardBackgroundService> _logger;

    public JiraDashboardBackgroundService(
        IServiceScopeFactory scopeFactory,
        JiraNotificationService notificationService,
        ILogger<JiraDashboardBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _notificationService = notificationService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Attendi 10s per lasciare spazio al boot dell'app e al ChatService (che aspetta 5s)
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(60));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PollDashboardAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il polling del Dashboard");
            }

            try
            {
                await timer.WaitForNextTickAsync(stoppingToken);
            }
            catch (OperationCanceledException) { break; }
        }

        _logger.LogInformation("JiraDashboardBackgroundService terminato.");
    }

    private async Task PollDashboardAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var queryBuilder = scope.ServiceProvider.GetRequiredService<JqlFilterBuilder>();
        var readClient = scope.ServiceProvider.GetRequiredService<JiraReadClient>();

        // Recupera tutte le issue senza filtri aggiuntivi (stesso approccio del dashboard di default)
        // Usiamo un filtro neutro: solo i progetti configurati, senza restrizioni su status/assignee
        var jql = queryBuilder.BuildDashboardQuery(new Model.Filter());

        var issues = await readClient.GetIssuesAsync(jql);

        _notificationService.NotifyDashboardIssuesUpdated(issues.ToList());

        _logger.LogDebug("JiraDashboardBackgroundService: poll completato, {Count} issue trovate.", issues.Count);
    }
}
