using Planner.Domain;

namespace Planner.Model;

public record TransitionModel
{
    public string Id { get; init; }
    public string Name { get; init; }
    public StatusModel TargetStatus { get; init; }

    public TransitionModel(Transition dto)
    {
        Id = dto.Id;
        Name = dto.Name;
        TargetStatus = new(dto.To);
    }
}