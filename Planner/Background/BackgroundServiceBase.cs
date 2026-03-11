using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Planner.Background;

/// <summary>
/// Base class for periodic background services using Rx.NET.
/// </summary>
public abstract class BackgroundServiceBase(int intervalInSeconds, ILogger<BackgroundServiceBase> logger) : BackgroundService
{
    /// <summary>
    /// Executes the core logic of the background service.
    /// </summary>
    protected abstract Task RunAsync(CancellationToken stoppingToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var serviceName = GetType().Name;

        try
        {
            await Observable
                .Timer(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(intervalInSeconds))
                .Select(_ => Observable.FromAsync(async ct =>
                {
                    try
                    {
                        await RunAsync(ct);
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        logger.LogError(ex, "An error occurred during the polling execution of {ServiceName}.", serviceName);
                    }
                }))
                
                // Concat assicura che se un'esecuzione di RunAsync impiega molto tempo, la successiva attenderà che finisca.
                // Evita sovrapposizioni.
                .Concat() 
                .ToTask(stoppingToken);
        }
        catch (TaskCanceledException)
        {
            // Questo blocco scatta normalmente quando l'applicazione viene spenta
        }

        logger.LogInformation("{ServiceName} has terminated.", serviceName);
    }
}