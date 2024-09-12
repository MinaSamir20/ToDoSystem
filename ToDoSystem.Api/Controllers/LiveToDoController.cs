using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ToDoSystem.Api.Base;
using ToDoSystem.Domain.Entities;

namespace ToDoSystem.Api.Controllers
{
    [Route("liveTodo")]
    public class LiveToDoController : AppControllerBase
    {
        private readonly HttpClient _httpClient;
        public LiveToDoController()
        {
            _httpClient = new HttpClient();
        }
        [HttpGet, Authorize]
        public async Task<IActionResult> GetData([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var apiUrl = "https://jsonplaceholder.typicode.com/todos";
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var todos = JsonConvert.DeserializeObject<List<Todo>>(data);
                int totalItems = todos!.Count;
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var paginatedData = todos.Skip((page - 1) * pageSize).Take(pageSize);

                var result = new
                {
                    Data = paginatedData,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    TotalItems = totalItems,
                    Meta = new { Count = totalItems },
                    PageSize = pageSize,
                    HasPreviousPage = page > 1,
                    hasNextPage = page < totalPages,
                    succeeded = true
                };
                return Ok(result);
            }
            else
            {
                return StatusCode((int)response.StatusCode);
            }
        }
    }
}
