using Planner.Domain;
using Planner.Extensions;

namespace Planner.Model;

public record StatusModel
{
    public string Icon { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }

    public StatusModel(Status status)
    {
        Icon = status.IconUrl; 
        Name = status.Name.ToTitleCase();
        Category = status.StatusCategory?.Name ?? string.Empty;
    }
}