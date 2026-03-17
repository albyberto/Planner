using System.Collections.Immutable;
using Planner.Domain;
using Planner.Extensions;
using Type = Planner.Domain.Type;

namespace Planner.Model;

public record TypeModel
{
    public string Value { get; init; }
    public string Name { get; init; }
    public string IconUrl { get; init; }
    public ImmutableList<StatusModel> Statuses { get; init; }

    public TypeModel(Type type)
    {
        Value = type.Id;
        Name = type.Name.ToTitleCase();
        IconUrl = type.IconUrl ?? string.Empty;
        
        Statuses = type.Statuses.Select(status => new StatusModel(status)).ToImmutableList() ?? [];
    }
    
    public override string ToString() => Name;
}