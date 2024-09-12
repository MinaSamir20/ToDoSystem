#nullable disable
namespace ToDoSystem.Application.Features.Todos.Responces
{
    public class GetTodoPaginatedResponse(int id, string title)
    {
        public int Id { get; set; } = id;
        public string Title { get; set; } = title;
    }
}
