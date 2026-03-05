using Planner.Domain;
using Planner.Extensions;

namespace Planner.Model;

public record FixVersionModel
{
    public string Value { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }

    public FixVersionModel(FixVersion version)
    {
        Value = version.Id;
        Name = version.Name.ToTitleCase();
        Description = version.Description;
    }
}