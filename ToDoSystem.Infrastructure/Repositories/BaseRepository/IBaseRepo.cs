using System.Linq.Expressions;

namespace ToDoSystem.Infrastructure.Repositories.BaseRepository
{
    public interface IBaseRepo<T> where T : class
    {
        Task<T> CreateAsync(T entity, int UserId);
        Task<T> GetAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? criteria);
        IQueryable<T> GetTableNoTracking();
        Task<T> UpdateAsync(T entity, int UserId);
        Task<T> DeleteAsync(int id);
    }
}
