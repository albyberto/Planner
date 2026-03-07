using Planner.Domain;
using Planner.Extensions;

namespace Planner.Model;

public record FixVersionModel
{
    public string Value { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public FixVersionModel(FixVersion version)
    {
        Value = version.Id;
        Name = version.Name.ToTitleCase();
        Description = version.Description;
    }
}