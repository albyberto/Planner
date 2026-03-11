using System.Collections.Immutable;
using System.Linq;
using Planner.Domain;
using Planner.Extensions;

namespace Planner.Model;

public record TypeModel
{
    public string Value { get; init; }
    public string Name { get; init; }
    public string IconUrl { get; init; }
    public ImmutableList<StatusModel> Statuses { get; init; }

    public TypeModel(IssueType type)
    {
        Value = type.Id;
        Name = type.Name.ToTitleCase();
        IconUrl = type.IconUrl ?? string.Empty;
        
        Statuses = type.Statuses.Select(status => new StatusModel(status)).ToImmutableList();
    }
}