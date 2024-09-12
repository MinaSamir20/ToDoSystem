using MediatR;
using ToDoSystem.Application.Features.Todos.Responces;
using ToDoSystem.Application.Warppers;

namespace ToDoSystem.Application.Features.Todos.Models
{
    public class GetTodoPaginatedList : IRequest<PaginatedResult<GetTodoPaginatedResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
