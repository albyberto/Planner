using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Planner.Clients;
using Planner.Model;
using Planner.Options;
using JQLBuilder;

namespace Planner.Services;

public class JiraChatBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly JiraNotificationService _notificationService;
    private readonly ILogger<JiraChatBackgroundService> _logger;

    private List<IssueModel> _lastKnownIssues = [];
    private bool _isFirstRun = true;

    public JiraChatBackgroundService(
        IServiceScopeFactory scopeFactory,
        JiraNotificationService notificationService,
        ILogger<JiraChatBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _notificationService = notificationService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Attendi prima di iniziare per far sì che l'app avvii tutto il resto
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try 
            {
                await PollJiraChatAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il polling di Jira Chat nel background service");
            }
            
            try 
            {
                await timer.WaitForNextTickAsync(stoppingToken);
            }
            catch (OperationCanceledException) { break; }
        }
    }

    private async Task PollJiraChatAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var filterOptions = scope.ServiceProvider.GetRequiredService<IOptions<JiraFilterOptions>>().Value;
        
        var meOptions = filterOptions.Me;
        if (string.IsNullOrWhiteSpace(meOptions.Email)) return;

        var readClient = scope.ServiceProvider.GetRequiredService<JiraReadClient>();

        // Recupera displayName dell'utente in modo safe
        string meDisplayName = meOptions.Email;
        try
        {
            var users = new List<UserModel>();
            var projects = readClient.GetProjects();
            foreach (var proj in projects)
            {
                users.AddRange(await proj.GetAssigneesAsync());
            }
            var meUserObj = users.DistinctBy(u => u.AccountId).FirstOrDefault(u => string.Equals(u.EmailAddress, meOptions.Email, StringComparison.OrdinalIgnoreCase));
            if (meUserObj != null && !string.IsNullOrWhiteSpace(meUserObj.DisplayName))
            {
                meDisplayName = meUserObj.DisplayName;
            }
        }
        catch { /* Fallback a email */ }

        // Build Query per recuperare esattamente quello che serve alla chat indipendentemente dai filtri visivi ui
        var meUser = (JQLBuilder.Types.JqlTypes.JqlHistoricalJqlUser)meOptions.Email;
        var chatQuery = JQLBuilder.JqlBuilder.Query
            .Where(f => f.User.Assignee == meUser | f.User.Reporter == meUser | f.Text.Comment.Contains(meDisplayName));
        
        var jqlString = chatQuery + " ORDER BY updated DESC";

        var fetchedIssues = await readClient.GetIssuesAsync(jqlString);
        
        var validIssues = fetchedIssues.Where(i => 
            i.Comments.Any() &&
            ((i.Assignee?.EmailAddress?.Equals(meOptions.Email, StringComparison.OrdinalIgnoreCase) == true) ||
             i.Comments.Any(c => c.Body.Contains(meDisplayName, StringComparison.OrdinalIgnoreCase) || 
                                 c.Author.EmailAddress?.Equals(meOptions.Email, StringComparison.OrdinalIgnoreCase) == true))
        ).ToList();

        CheckForNewCommentsAndNotify(_lastKnownIssues, validIssues, meOptions.Email, _isFirstRun);

        _lastKnownIssues = validIssues;
        _isFirstRun = false;
        
        _notificationService.NotifyIssuesUpdated(validIssues);
    }
    
    private void CheckForNewCommentsAndNotify(List<IssueModel> oldIssues, List<IssueModel> newIssues, string myEmail, bool isFirstRun)
    {
        int newCommentCount = 0;
        string? lastIssueKey = null;
        string? lastCommentAuthor = null;

        foreach (var newIssue in newIssues)
        {
            var oldIssue = oldIssues.FirstOrDefault(i => i.Key == newIssue.Key);
            if (oldIssue == null)
            {
                var newComments = newIssue.Comments.Where(c => c.Created > DateTime.Now.AddMinutes(-10)).ToList();
                newCommentCount += newComments.Count;
                if (newComments.Any())
                {
                    lastIssueKey = newIssue.Key;
                    lastCommentAuthor = newComments.Last().Author.DisplayName;
                }
            }
            else
            {
                var oldCommentIds = oldIssue.Comments.Select(c => c.Id).ToHashSet();
                var newlyAddedComments = newIssue.Comments.Where(c => !oldCommentIds.Contains(c.Id)).ToList();
                
                var otherPeopleComments = newlyAddedComments.Where(c => c.Author.EmailAddress != null && !c.Author.EmailAddress.Equals(myEmail, StringComparison.OrdinalIgnoreCase)).ToList();

                newCommentCount += otherPeopleComments.Count;
                if (otherPeopleComments.Any())
                {
                    lastIssueKey = newIssue.Key;
                    lastCommentAuthor = otherPeopleComments.Last().Author.DisplayName;
                }
            }
        }

        if (newCommentCount > 0 && !isFirstRun)
        {
            _notificationService.NotifyNewComments(newCommentCount, lastCommentAuthor ?? "Sconosciuto", lastIssueKey ?? "?");
        }
    }
}
