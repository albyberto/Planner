using Planner.Domain;
using Planner.Extensions;

namespace Planner.Model;

public record TransitionModel
{
    public string To { get; init; }

    public TransitionModel(Transition status)
    {
        To = status.To.Name.ToTitleCase();
    }
}