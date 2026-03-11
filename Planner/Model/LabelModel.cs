using Planner.Extensions;

namespace Planner.Model;

public record LabelModel(string Value) : IMultiSelect
{
    public string Name { get; init; } = Value.ToTitleCase();
}