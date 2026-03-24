using Planner.Clients.Domain;
using Planner.Extensions;

namespace Planner.Model;

public record FixVersionModel
{
    public string Value { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public DateOnly? ReleaseDate { get; set; }

    public FixVersionModel() {}

    public FixVersionModel(FixVersion version)
    {
        Value = version.Id;
        Name = version.Name.ToTitleCase();
        Description = version.Description ?? string.Empty;
        StartDate = version.StartDate;
        ReleaseDate = version.ReleaseDate;
    }

    public static readonly FixVersionModel Unassigned = new() { Value = "-1", Name = "Unassigned", Description = "Senza Fix Version" };
}