#nullable disable
using ToDoSystem.Domain.Entities.Identity;

namespace ToDoSystem.Domain.Entities
{
    public class Todo : BaseEntity
    {
        public string Title { get; set; }
        public bool Completed { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
    }
}
