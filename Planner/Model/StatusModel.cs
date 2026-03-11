using Planner.Domain;
using Planner.Extensions;

namespace Planner.Model;

public record StatusModel
{
    public string Name { get; init; }
    public string Icon { get; init; }
    public string Category { get; init; }

    public StatusModel(Status status)
    {
        Name = status.Name.ToTitleCase();
        Icon = status.IconUrl; 
        Category = status.StatusCategory?.Name ?? string.Empty;
    }
}