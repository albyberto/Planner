namespace Planner.Model;

public record LabelModel
{
    public string Value { get; init; }
    public string Label { get; init; }

    public LabelModel(string label)
    {
        Value = label;
        Label = label;
    }
}