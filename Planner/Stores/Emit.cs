namespace Planner.Stores;

public record Emit<T>(Guid Key, T Value);