namespace Planner.Components.Shared.Filters.Model;

public enum DateField
{
    Created,
    Updated,
    Resolved,
    DueDate
}

public enum DatePreset
{
    None,
    Today,
    Yesterday,
    LastWeek,
    LastMonth,
    ThisWeek, 
    Last7Days
}
