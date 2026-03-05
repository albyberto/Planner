using Planner.Domain;
using Planner.Extensions;

namespace Planner.Model;

public record StatusModel
{
    public string Icon { get; init; }
    public string Name { get; init; }

    public StatusModel(Status status)
    {
        Icon = status.IconUrl; 
        Name = status.Name.ToTitleCase();
    }
}