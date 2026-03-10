using System.Text.Json.Serialization;

namespace Planner.Domain;

public record Transition(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("to")] Status To
);

public record TransitionsResponse(
    [property: JsonPropertyName("transitions")] IReadOnlyList<Transition> Transitions
);

