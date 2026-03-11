// using Microsoft.Extensions.Options;
// using Planner.Clients;
// using Planner.Infrastructure.Clients;
// using Planner.Options;
//
// namespace Planner.Background;
//
// public class DashboardBackgroundService(JiraReadClient client, IOptions<JiraFilterOptions> options, ILogger<DashboardBackgroundService> logger) 
//     : BackgroundServiceBase(60, logger)
// {
//     protected override async Task RunAsync(CancellationToken stoppingToken)
//     {
//     
//     }
// }