using AutoMapper;
using MediatR;
using ToDoSystem.Application.Bases;
using ToDoSystem.Application.Features.Todos.Models;
using ToDoSystem.Application.Services.ToDo;
using ToDoSystem.Domain.Entities;

namespace ToDoSystem.Application.Features.Todos.Handler
{
    public class TodoCommandHandler(IMapper mapper, ITodoService service) : ResponseHandler,
        IRequestHandler<CreateTodo, Response<string>>,
        IRequestHandler<UpdateTodo, Response<string>>,
        IRequestHandler<DeleteTodo, Response<string>>
    {
        private readonly IMapper _mapper = mapper;
        private readonly ITodoService _service = service;

        public async Task<Response<string>> Handle(CreateTodo request, CancellationToken cancellationToken)
        {
            var todo = _mapper.Map<Todo>(request);
            var result = await _service.AddEdit(todo, request.UserId);
            if (result == "Success") return Success<string>("Created Successfully");
            else return BadRequest<string>("Something Wrong");
        }

        public async Task<Response<string>> Handle(UpdateTodo request, CancellationToken cancellationToken)
        {
            var todo = _mapper.Map<Todo>(request);
            var result = await _service.AddEdit(todo, request.UserId);
            if (result == "Success") return Success<string>("Updated Successfully");
            else return BadRequest<string>("Something Wrong");

        }

        public async Task<Response<string>> Handle(DeleteTodo request, CancellationToken cancellationToken)
        {
            var result = await _service.Remove(request.ID);
            if (result == "Success") return Deleted<string>("Deleted Successfully");
            else return BadRequest<string>("Something Wrong");
        }
    }
}
