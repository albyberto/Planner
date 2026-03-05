using Planner.Domain;
using Planner.Extensions;

namespace Planner.Model;

public record IssueTypeModel
{
    public string Value { get; init; }
    public string Name { get; init; }

    public IssueTypeModel(IssueType type)
    {
        Value = type.Id;
        Name = type.Name.ToTitleCase();
    }
}