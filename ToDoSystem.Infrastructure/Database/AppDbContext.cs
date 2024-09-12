using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using ToDoSystem.Domain.Entities;
using ToDoSystem.Domain.Entities.Identity;

namespace ToDoSystem.Infrastructure.Database
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User, IdentityRole<int>, int>(options)
    {
        public DbSet<Todo> Todos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
