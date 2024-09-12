#nullable disable
namespace ToDoSystem.Application.Features.Todos.Responces
{
    public class GetTodoResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Completed { get; set; }
    }
}
