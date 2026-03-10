namespace Planner.Background;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Base class for periodic background services.
/// </summary>
public abstract class BackgroundServiceBase(int interval, ILogger<BackgroundServiceBase> logger) : BackgroundService
{
    /// <summary>
    /// Executes the core logic of the background service.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token triggered when the application is shutting down.</param>
    protected abstract Task RunAsync(CancellationToken stoppingToken);
        
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Initial delay to allow the application to fully start before running background tasks
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        // Set the polling interval (e.g., 60 seconds)
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(interval));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Execute the specific logic implemented by the derived class
                await RunAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // The application is shutting down, exit the loop gracefully
                break;
            }
            catch (Exception ex)
            {
                // Log the error using the actual type name of the derived class
                logger.LogError(ex, "An error occurred during the polling execution of {ServiceName}.", GetType().Name);
            }

            try
            {
                // Wait for the next timer tick before running the loop again
                await timer.WaitForNextTickAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Cancellation was requested while waiting, exit gracefully
                break;
            }
        }

        logger.LogInformation("{ServiceName} has terminated.", GetType().Name);
    }
}