using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Planner.Domain.Responses;

public record TransitionsResponse
{
    [JsonPropertyName("transitions")]
    public ImmutableArray<Transition> Transitions { get; init; } = [];
}