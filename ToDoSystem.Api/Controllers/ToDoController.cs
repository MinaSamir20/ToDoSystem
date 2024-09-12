using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoSystem.Api.Base;
using ToDoSystem.Application.Features.Todos.Models;
using ToDoSystem.Application.Services.Auth;

namespace ToDoSystem.Api.Controllers
{
    [Authorize]
    [Route("api/todo")]
    public class ToDoController(IAuthService service) : AppControllerBase
    {
        private readonly IAuthService _service = service;

        private async Task<int> GetCurrentLoginUserId()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
                return 0;
            var userId = await _service.GetUserId(username);
            return userId;
        }

        [HttpPost, Route("todo")]
        public async Task<IActionResult> Create(CreateTodo command)
        {
            command.UserId = await GetCurrentLoginUserId();
            return command.UserId == 0 ? Unauthorized("User Is Not Authorized") : NewResult(await Mediator.Send(command));
        }

        [HttpPut, Route("todo")]
        public async Task<IActionResult> Update(UpdateTodo command)
        {
            command.UserId = await GetCurrentLoginUserId();
            return command.UserId == 0 ? Unauthorized("User Is Not Authorized") : NewResult(await Mediator.Send(command));
        }

        [HttpDelete, Route("todo")]
        public async Task<IActionResult> Delete(DeleteTodo command) => NewResult(await Mediator.Send(command));

        [HttpGet, Route("todo/{id}")]
        public async Task<IActionResult> Get(int id) => NewResult(await Mediator.Send(new GetTodoDetails(id)));

        [HttpGet, Route("todos")]
        public async Task<IActionResult> Get() => NewResult(await Mediator.Send(new GetTodoList()));

        [HttpGet, Route("getTodoPagenated")]
        public async Task<IActionResult> Paginated([FromQuery] GetTodoPaginatedList query) => Ok(await Mediator.Send(query));
    }
}
