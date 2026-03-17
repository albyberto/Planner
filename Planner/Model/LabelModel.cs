using Planner.Components.Shared.Filters.Model;
using Planner.Extensions;

namespace Planner.Model;

public record LabelModel(string Value)
{
    public string Name { get; init; } = Value.ToTitleCase();
    
    public override string ToString() => Name;
}