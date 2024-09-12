using ToDoSystem.Domain.Entities;

namespace ToDoSystem.Application.Services.ToDo
{
    public interface ITodoService
    {
        Task<string> AddEdit(Todo todo, int userId);
        Task<Todo> Get(int id);
        IQueryable<Todo> GetPaginated();
        Task<IEnumerable<Todo>> Get();
        Task<string> Remove(int id);

    }
}
