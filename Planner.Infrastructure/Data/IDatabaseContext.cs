namespace Planner.Infrastructure.Data;

public interface IDatabaseContext
{
    Task<IEnumerable<T>> GetAsync<T>() where T : class;
    Task SaveChangesAsync<T>(IEnumerable<T> entities) where T : class;
}