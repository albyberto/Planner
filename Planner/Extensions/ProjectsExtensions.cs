using Planner.Model;

namespace Planner.Extensions;

public static class ProjectExtensions
{
    public static async Task<List<UserModel>> FindAssigneesByEmailsAsync(this IEnumerable<ProjectModel> projects, params string[] emails)
    {
        var foundUsers = new List<UserModel>();
        
        if (emails.Length == 0) return foundUsers;

        var remainingEmails = new HashSet<string>(emails, StringComparer.OrdinalIgnoreCase);

        foreach (var project in projects)
        {
            var assignees = await project.GetAssigneesAsync();

            foreach (var user in assignees.Where(u => !string.IsNullOrEmpty(u.EmailAddress) && remainingEmails.Contains(u.EmailAddress.ToLowerInvariant())))
            {
                foundUsers.Add(user);
                remainingEmails.Remove(user.EmailAddress);

                if (remainingEmails.Count == 0) return foundUsers;
            }
        }

        return foundUsers;
    }
}