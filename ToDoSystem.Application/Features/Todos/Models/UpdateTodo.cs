#nullable disable
using MediatR;
using ToDoSystem.Application.Bases;

namespace ToDoSystem.Application.Features.Todos.Models
{
    public class UpdateTodo : IRequest<Response<string>>
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public bool Completed { get; set; }
        public int UserId { get; set; } = 0;
    }
}
