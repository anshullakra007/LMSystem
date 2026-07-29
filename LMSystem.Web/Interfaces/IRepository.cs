using System.Linq.Expressions;

namespace LMSystem.Web.Interfaces;

public interface IRepository<T> where T : class
{
    IQueryable<T> AsQueryable();
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task SaveChangesAsync();
}
