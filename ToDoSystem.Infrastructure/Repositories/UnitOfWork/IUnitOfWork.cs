using ToDoSystem.Domain.Entities;
using ToDoSystem.Infrastructure.Repositories.BaseRepository;

namespace ToDoSystem.Infrastructure.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepo<Todo> Todos { get; }
        int Complete();
    }
}
