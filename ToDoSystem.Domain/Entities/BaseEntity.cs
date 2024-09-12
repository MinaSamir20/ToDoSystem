#nullable disable
namespace ToDoSystem.Domain.Entities
{
    public class BaseEntity
    {
        public int ID { get; set; }
        public int CreatorUserId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public int ModeficationUserId { get; set; }
        public DateTime ModifiedOn { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
}
