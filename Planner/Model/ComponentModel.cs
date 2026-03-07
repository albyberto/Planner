using Planner.Domain;
using Planner.Extensions;

namespace Planner.Model;

public record ComponentModel : IMultiSelect
{
    public string Value { get; set; }
    public string Name { get; set; }

    public ComponentModel(Component component)
    {
        Value = component.Id;
        Name = component.Name.ToTitleCase();
    }
}