using ToDoSystem.Domain.Entities;
using ToDoSystem.Infrastructure.Repositories.UnitOfWork;

namespace ToDoSystem.Application.Services.ToDo
{
    public class TodoService(IUnitOfWork unit) : ITodoService
    {
        private readonly IUnitOfWork _unit = unit;
        public async Task<string> AddEdit(Todo todo, int userId)
        {
            if (userId == 0)
                return "Invalid User";

            var result = await _unit.Todos.GetAsync(todo.ID);

            if (result is null)
                await _unit.Todos.CreateAsync(todo, userId);
            else
                await _unit.Todos.UpdateAsync(todo, userId);
            _unit.Complete();
            return "Success";

        }

        public Task<Todo> Get(int id)
        {
            return _unit.Todos.GetAsync(id);
        }

        public async Task<IEnumerable<Todo>> Get()
        {
            return await _unit.Todos.GetAllAsync(null);
        }

        public IQueryable<Todo> GetPaginated()
        {
            return _unit.Todos.GetTableNoTracking();
        }

        public async Task<string> Remove(int id)
        {
            var todo = await _unit.Todos.DeleteAsync(id);
            if (todo is null) return "Error";
            _unit.Complete();
            return "Success";

        }
    }
}
