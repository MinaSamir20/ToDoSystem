using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoSystem.Domain.Entities;

namespace ToDoSystem.Infrastructure.Configrations
{
    public class TodoConfig : IEntityTypeConfiguration<Todo>
    {
        public void Configure(EntityTypeBuilder<Todo> builder)
        {
            builder.HasKey(t => t.ID);
            builder.ToTable("Todos");

            builder.HasOne(t => t.User)
                .WithMany(u => u.Todos)
                .HasForeignKey(t => t.UserID);

            builder.Property(t => t.Title).HasColumnName("Title").HasMaxLength(200);
            builder.Property(t => t.Completed).HasColumnName("Completed").HasDefaultValue(false);
            builder.Property(t => t.IsDeleted).HasColumnName("IsDeleted").HasDefaultValue(false);
        }
    }
}
