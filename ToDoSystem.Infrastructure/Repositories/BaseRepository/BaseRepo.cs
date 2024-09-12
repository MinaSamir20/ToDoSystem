using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ToDoSystem.Domain.Entities;
using ToDoSystem.Infrastructure.Database;

namespace ToDoSystem.Infrastructure.Repositories.BaseRepository
{
    public class BaseRepo<T>(AppDbContext db) : IBaseRepo<T> where T : BaseEntity
    {
        private readonly AppDbContext _db = db;

        public async Task<T> CreateAsync(T entity, int UserId)
        {
            entity.CreatedOn = DateTime.Now;
            entity.CreatorUserId = UserId;
            entity.ModifiedOn = DateTime.Now;
            entity.ModeficationUserId = UserId;
            await _db.Set<T>().AddAsync(entity);
            return entity;
        }

        public async Task<T> DeleteAsync(int id)
        {
            var entity = await _db.Set<T>().Where(a => !a.IsDeleted && a.ID == id).SingleOrDefaultAsync();
            if (entity != null)
            {
                entity.DeletedDate = DateTime.Now;
                entity.IsDeleted = true;
            }
            return entity!;
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? criteria)
        {
            var query = GetTableNoTracking();
            if (criteria is not null) query = query.Where(criteria);
            return (await query.ToListAsync())!;
        }

        public async Task<T> GetAsync(int id) => (await GetTableNoTracking().Where(a => a.ID == id).SingleOrDefaultAsync())!;

        public IQueryable<T> GetTableNoTracking() => _db.Set<T>().Where(a => a.IsDeleted == false).AsNoTracking().AsQueryable();
        public IEnumerable<T> GetTableTracking() => _db.Set<T>().Where(a => a.IsDeleted == false).AsEnumerable();

        public async Task<T> UpdateAsync(T entity, int UserId)
        {
            var t = await _db.Set<T>().Where(a => !a.IsDeleted && a.ID == entity.ID).SingleOrDefaultAsync();
            if (t is not null)
            {
                entity.ModifiedOn = DateTime.Now;
                entity.ModeficationUserId = UserId;
                _db.Entry(t).CurrentValues.SetValues(entity);
            }
            return entity;
        }
    }
}
