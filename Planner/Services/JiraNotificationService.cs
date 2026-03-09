using Planner.Model;

namespace Planner.Services;

public class JiraNotificationService
{
    public event Action<int, string, string>? OnNewComments;
    public event Action<List<IssueModel>>? OnIssuesUpdated;

    public void NotifyNewComments(int count, string author, string issueKey)
    {
        OnNewComments?.Invoke(count, author, issueKey);
    }
    
    public void NotifyIssuesUpdated(List<IssueModel> issues)
    {
        OnIssuesUpdated?.Invoke(issues);
    }
}
