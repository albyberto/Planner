using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Planner.Domain;

public record Type
{
    [JsonPropertyName("self")]
    public string? Self { get; init; }

    [JsonPropertyName("id")]
    public required string Id { get; init; }

    // Nullable: Presente SOLO nel JSON JQL. Manca completamente nel JSON Project Statuses.
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    // Nullable: Presente SOLO nel JSON JQL. Manca in Project Statuses.
    [JsonPropertyName("iconUrl")]
    public string? IconUrl { get; init; }

    // Required: Il nome (es. "Epic", "Bug") è sempre presente.
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    // Nullable: Presente in entrambi, ma tipicamente un booleano API è più sicuro come nullable.
    [JsonPropertyName("subtask")]
    public bool? Subtask { get; init; }

    // Nullable: Presente SOLO nel JSON JQL. Manca in Project Statuses.
    [JsonPropertyName("avatarId")]
    public int? AvatarId { get; init; }

    // Nullable: Presente SOLO nel JSON JQL. Manca in Project Statuses.
    [JsonPropertyName("hierarchyLevel")]
    public int? HierarchyLevel { get; init; }

    // Assente nel JQL, presente SOLO in Project Statuses. Inizializzato a lista vuota così non è mai null.
    [JsonPropertyName("statuses")]
    public ImmutableArray<Status> Statuses { get; init; } = [];
}