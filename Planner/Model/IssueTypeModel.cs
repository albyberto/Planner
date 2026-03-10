using Planner.Domain;
using Planner.Extensions;
using System.Collections.Immutable;

namespace Planner.Model;

public record IssueTypeModel
{
    public string Value { get; init; }
    public string Name { get; init; }
    public string IconUrl { get; init; }
    
    public ImmutableList<StatusModel> Statuses { get; init; }

    public IssueTypeModel(IssueType type)
    {
        Value = type.Id;
        Name = type.Name.ToTitleCase();
        IconUrl = type.IconUrl ?? string.Empty;
        
        Statuses = type.Statuses.Select(s => new StatusModel(s)).ToImmutableList();
    }
}