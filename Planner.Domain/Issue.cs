using System.Text.Json.Serialization;

namespace Planner.Domain.Issue; // Assicurati che i tipi come User, Project, ecc. siano visibili qui

public record Issue
{
    // Required: ID e Key ci sono SEMPRE in una issue
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("key")]
    public required string Key { get; init; }

    // Nullable
    [JsonPropertyName("self")]
    public string? Self { get; init; }

    [JsonPropertyName("expand")] 
    public string? Expand { get; init; }

    // Nullable: Se fai una query JQL chiedendo "?fields=none", Fields non ci sarà!
    [JsonPropertyName("fields")]
    public Fields? Fields { get; init; }
}

public record Fields
{
    // --- CAMPI TESTUALI E DATE ---
    [JsonPropertyName("summary")] 
    public string? Summary { get; init; }

    [JsonPropertyName("created")]
    [JsonConverter(typeof(JiraDateOnlyConverter))]
    public DateOnly? Created { get; init; }

    [JsonPropertyName("updated")]
    [JsonConverter(typeof(JiraDateOnlyConverter))]
    public DateOnly? Updated { get; init; }

    [JsonPropertyName("duedate")]
    public DateOnly? DueDate { get; init; }

    [JsonPropertyName("customfield_10117")]
    public DateOnly? StartDate { get; init; }

    [JsonPropertyName("customfield_10118")]
    public DateOnly? EndDate { get; init; }

    // --- COLLEZIONI (Inizializzate vuote per evitare NullReference) ---
    [JsonPropertyName("components")] 
    public IReadOnlyList<Component> Components { get; init; } = [];

    [JsonPropertyName("labels")] 
    public IReadOnlyList<string> Labels { get; init; } = [];
    
    [JsonPropertyName("fixVersions")] 
    public IReadOnlyList<FixVersion> FixVersions { get; init; } = [];

    [JsonPropertyName("issuetype")]
    public Type? Type { get; init; }

    [JsonPropertyName("status")]
    public Status? Status { get; init; }

    [JsonPropertyName("project")]
    public Project? Project { get; init; }

    [JsonPropertyName("assignee")]
    public User? Assignee { get; init; }

    [JsonPropertyName("timetracking")]
    public TimeTracking? TimeTracking { get; init; }

    [JsonPropertyName("worklog")]
    public Worklog? Worklog { get; init; }

    [JsonPropertyName("comment")]
    public Comment? Comment { get; init; } // <-- Corretto: questo è il contenitore paginato!
}