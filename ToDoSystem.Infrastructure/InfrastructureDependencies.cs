using Microsoft.Extensions.DependencyInjection;
using ToDoSystem.Infrastructure.Repositories.UnitOfWork;

namespace ToDoSystem.Infrastructure
{
    public static class InfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
