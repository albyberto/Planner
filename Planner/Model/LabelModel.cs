namespace Planner.Model;

public record LabelModel
{
    public string Value { get; set; }
    public string Label { get; set; }

    public LabelModel(string label)
    {
        Value = label;
        Label = label;
    }
}