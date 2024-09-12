#nullable disable
using MediatR;
using ToDoSystem.Application.Bases;

namespace ToDoSystem.Application.Features.Todos.Models
{
    public class DeleteTodo : IRequest<Response<string>>
    {
        public int ID { get; set; }
    }
}
