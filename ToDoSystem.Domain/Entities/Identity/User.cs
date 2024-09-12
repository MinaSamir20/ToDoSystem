using Microsoft.AspNetCore.Identity;

namespace ToDoSystem.Domain.Entities.Identity
{
    public class User : IdentityUser<int>
    {
        public IEnumerable<Todo>? Todos { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }
    }
}
