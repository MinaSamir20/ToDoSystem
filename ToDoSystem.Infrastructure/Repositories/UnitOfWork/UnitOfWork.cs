using ToDoSystem.Domain.Entities;
using ToDoSystem.Infrastructure.Database;
using ToDoSystem.Infrastructure.Repositories.BaseRepository;

namespace ToDoSystem.Infrastructure.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        public IBaseRepo<Todo> Todos { get; private set; }

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            Todos = new BaseRepo<Todo>(_db);
        }

        public int Complete() => _db.SaveChanges();

        public void Dispose() => _db.Dispose();
    }
}
