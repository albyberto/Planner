using Planner.Domain;
using Planner.Extensions;

namespace Planner.Model;

public record IssueTypeModel
{
    public string Value { get; set; }
    public string Name { get; set; }
    public string IconUrl { get; set; }

    public IssueTypeModel(IssueType type)
    {
        Value = type.Id;
        Name = type.Name.ToTitleCase();
        IconUrl = type.IconUrl;
    }
}