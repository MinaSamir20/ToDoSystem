#nullable disable
using MediatR;
using ToDoSystem.Application.Bases;
using ToDoSystem.Application.Features.Todos.Responces;

namespace ToDoSystem.Application.Features.Todos.Models
{
    public class GetTodoList : IRequest<Response<IEnumerable<GetTodoResponse>>>
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public bool Completed { get; set; }
        public int UserID { get; set; }
    }
}
