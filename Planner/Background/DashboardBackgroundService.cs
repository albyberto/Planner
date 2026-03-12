using System.Reactive.Linq;
using Planner.Infrastructure.Clients;
using Planner.Model;
using Planner.Services;
using Planner.Stores;

namespace Planner.Background;

public class DashboardBackgroundService(FilterStore filterStore, IssueStore issueStore, JiraReadClient readClient, JqlFilterBuilder queryBuilder, ILogger<DashboardBackgroundService> logger) : BackgroundServiceBase(60, logger)
{
    protected override async Task RunAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("DashboardBackgroundService in ascolto dei filtri...");

        var subscription = filterStore.ObserveGlobal()
            .Where(emit => emit.Value is not null)
            .GroupBy(emit => emit.Key)
            .SelectMany(group => group
                .Throttle(TimeSpan.FromMilliseconds(500)) // Debounce di 500ms: aspetta che l'utente smetta di cliccare i filtri prima di partire
                .SelectMany(async emit => await ProcessFilterAsync(emit, stoppingToken))
            )
            .Subscribe(
                _ => { /* Elaborazione terminata con successo per l'evento */ },
                ex => logger.LogCritical(ex, "Errore fatale nello stream globale dei filtri")
            );

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (TaskCanceledException)
        {
            // Spegnimento grazioso (Ctrl+C o stop dell'app)
        }
        finally
        {
            subscription.Dispose();
        }
    }

    private async Task<System.Reactive.Unit> ProcessFilterAsync(Emit<IssueSearchCriteria> emit, CancellationToken ct)
    {
        try
        {
            logger.LogDebug("Costruzione JQL per la Key: {Key}", emit.Key);
            
            // 1. Converti il filtro UI in JQL
            var jql = queryBuilder.Build(emit.Value!);

            // 2. Esegui la chiamata HTTP a Jira
            var issues = await readClient.GetIssuesAsync(jql, ct);

            // 3. Emetti i risultati verso lo store che alimenta la tabella/dashboard
            // Assumiamo che IssueStore accetti un IEnumerable/ImmutableArray e la stessa Key della vista
            issueStore.Emit(emit.Key, issues);
            
            logger.LogInformation("Recuperate {Count} issue per la Key: {Key}", issues.Count, emit.Key);
        }
        catch (Exception ex)
        {
            // Gestione dell'errore confinata al singolo Emit. Non fa crollare il BackgroundService.
            logger.LogError(ex, "Errore di comunicazione con Jira per la Key {Key}", emit.Key);
            
            // Opzionale: potresti emettere un evento di errore allo store per mostrare uno Snackbar nella UI
            // issueStore.EmitError(emit.Key, "Errore durante il recupero dei dati da Jira.");
        }

        return System.Reactive.Unit.Default;
    }
}