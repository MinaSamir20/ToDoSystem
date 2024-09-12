#nullable disable
using MediatR;
using ToDoSystem.Application.Bases;

namespace ToDoSystem.Application.Features.Todos.Models
{
    public class CreateTodo : IRequest<Response<string>>
    {
        public string Title { get; set; }
        public bool Completed { get; set; }
        public int UserId { get; set; }
    }
}
