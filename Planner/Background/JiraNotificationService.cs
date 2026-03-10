using Planner.Model;

namespace Planner.Background;

/// <summary>
/// Singleton che funge da bus di eventi tra i background service e i componenti Blazor.
/// Espone eventi per notificare: nuovi commenti (Chat) e aggiornamenti delle issue (Dashboard).
/// </summary>
public class JiraNotificationService
{
    // ── Chat ────────────────────────────────────────────────────────────────
    /// <summary>Notifica quando arrivano nuovi commenti: (count, author, issueKey).</summary>
    public event Action<int, string, string>? OnNewComments;

    /// <summary>Notifica quando le issue della Chat vengono aggiornate dal polling.</summary>
    public event Action<List<IssueModel>>? OnChatIssuesUpdated;

    public void NotifyNewComments(int count, string author, string issueKey)
        => OnNewComments?.Invoke(count, author, issueKey);

    public void NotifyChatIssuesUpdated(List<IssueModel> issues)
        => OnChatIssuesUpdated?.Invoke(issues);

    // ── Dashboard ────────────────────────────────────────────────────────────
    /// <summary>Notifica quando le issue del Dashboard vengono aggiornate dal polling.</summary>
    public event Action<List<IssueModel>>? OnDashboardIssuesUpdated;

    public void NotifyDashboardIssuesUpdated(List<IssueModel> issues)
        => OnDashboardIssuesUpdated?.Invoke(issues);
}
