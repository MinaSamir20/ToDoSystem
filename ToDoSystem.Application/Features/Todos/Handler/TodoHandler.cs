using AutoMapper;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using ToDoSystem.Application.Bases;
using ToDoSystem.Application.Features.Todos.Models;
using ToDoSystem.Application.Features.Todos.Responces;
using ToDoSystem.Application.Services.ToDo;
using ToDoSystem.Application.Warppers;
using ToDoSystem.Domain.Entities;

namespace ToDoSystem.Application.Features.Todos.Handler
{
    public class TodoHandler(IMapper mapper, ITodoService service) : ResponseHandler,
        IRequestHandler<GetTodoList, Response<IEnumerable<GetTodoResponse>>>,
        IRequestHandler<GetTodoDetails, Response<GetTodoResponse>>,
        IRequestHandler<GetTodoPaginatedList, PaginatedResult<GetTodoPaginatedResponse>>

    {
        private readonly IMapper _mapper = mapper;
        private readonly ITodoService _service = service;
        public async Task<Response<IEnumerable<GetTodoResponse>>> Handle(GetTodoList request, CancellationToken cancellationToken)
        {
            var todos = await _service.Get();
            return todos.IsNullOrEmpty() ? NotFound<IEnumerable<GetTodoResponse>>("Not Found Any Items") : Success(_mapper.Map<IEnumerable<GetTodoResponse>>(todos));
        }

        public async Task<Response<GetTodoResponse>> Handle(GetTodoDetails request, CancellationToken cancellationToken)
        {
            var todo = await _service.Get(request.ID);

            return todo == null ? NotFound<GetTodoResponse>("This Item Is Not Found") : Success(_mapper.Map<GetTodoResponse>(todo));
        }

        public async Task<PaginatedResult<GetTodoPaginatedResponse>> Handle(GetTodoPaginatedList request, CancellationToken cancellationToken)
        {
            Expression<Func<Todo, GetTodoPaginatedResponse>> Expression = e => new GetTodoPaginatedResponse(e.ID, e.Title);
            var todos = _service.GetPaginated();
            var paginated = await todos.Select(Expression).ToPaginatedListAsync(request.PageNumber, request.PageSize);
            paginated.Meta = new { paginated.Data.Count };
            return paginated!;
        }
    }
}
