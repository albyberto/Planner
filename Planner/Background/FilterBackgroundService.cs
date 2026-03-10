namespace Planner.Background;

public class FilterBackgroundService(ILogger<FilterBackgroundService> logger)
    : BackgroundServiceBase(60, logger)
{
    protected override Task RunAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}