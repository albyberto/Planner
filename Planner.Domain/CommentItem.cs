using System.Text.Json.Serialization;

namespace Planner.Domain;

public record Comment
{
    [JsonPropertyName("id")] public string? Id { get; init; }
    [JsonPropertyName("author")] public User? Author { get; init; }
    [JsonPropertyName("created")] public string? Created { get; init; }
    
    // ECCOLO QUI: Intercettiamo il "body" e gli diciamo che è un documento di testo!
    [JsonPropertyName("body")] 
    public AdfDocument? Body { get; init; } 
}