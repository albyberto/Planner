using Planner.Extensions;

namespace Planner.Model;

public record LabelModel(string Value) : IMultiSelect
{
    public string Value { get; set; } = Value;
    public string Name { get; set; } = Value.ToTitleCase();
}