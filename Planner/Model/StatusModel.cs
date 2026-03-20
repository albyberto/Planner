using MudBlazor;
using Planner.Clients.Domain;
using Planner.Extensions;
using Planner.Model.Abstract;

namespace Planner.Model;

public record StatusModel : IColor
{
    public string Name { get; init; }
    public string? Icon { get; init; }
    public string CategoryName { get; init; }
    
    public Color Color { get; init; }

    public StatusModel(Status status)
    {
        Name = status.Name.ToTitleCase();
        Icon = status.IconUrl; 
        CategoryName = status.StatusCategory.Name;

        Color = status.StatusCategory.Id switch
        {
            2 => Color.Dark,    // Grigio scuro per "To Do"
            4 => Color.Info,    // Azzurro per "In Progress"
            3 => Color.Success, // Verde per "Done"
            _ => Color.Default  // Fallback
        };
    }
    
    public override string ToString() => Name;
}