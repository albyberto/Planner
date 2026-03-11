using Planner.Domain;
using Planner.Extensions;

namespace Planner.Model;

public record ComponentModel : IMultiSelect
{
    public string Value { get; init; }
    public string Name { get; init; }

    public ComponentModel(Component component)
    {
        Value = component.Id;
        Name = component.Name.ToTitleCase();
    }
}