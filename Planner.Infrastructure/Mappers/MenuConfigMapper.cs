using Planner.Infrastructure.Documents;
using Planner.Infrastructure.Domain;

namespace Planner.Infrastructure.Mappers;

public static class MenuConfigMapper
{
    public static MenuConfigDocument ToDocument(this MenuConfigItem item)
    {
        return new MenuConfigDocument(
            Id: item.Id,
            ProjectKey: item.ProjectKey,
            Assignees: item.Assignees,
            Created: item.Created,
            Updated: item.Updated
        );
    }

    public static MenuConfigItem ToItem(this MenuConfigDocument document)
    {
        return new MenuConfigItem
        {
            Id = document.Id,
            ProjectKey = document.ProjectKey,
            Assignees = document.Assignees,
            Created = document.Created,
            Updated = document.Updated ?? document.Created
        };
    }

    public static MenuConfigDocument Merge(this MenuConfigDocument existing, MenuConfigItem updated)
    {
        return existing with
        {
            ProjectKey = updated.ProjectKey,
            Assignees = updated.Assignees,
            Updated = DateTime.UtcNow
        };
    }
}
