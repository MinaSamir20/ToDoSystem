#nullable disable
using MediatR;
using ToDoSystem.Application.Bases;
using ToDoSystem.Application.Features.Todos.Responces;

namespace ToDoSystem.Application.Features.Todos.Models
{
    public class GetTodoDetails(int id) : IRequest<Response<GetTodoResponse>>
    {
        public int ID { get; set; } = id;
        public int UserID { get; set; }
    }
}
