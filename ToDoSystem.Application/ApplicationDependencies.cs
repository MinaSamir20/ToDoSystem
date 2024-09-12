using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ToDoSystem.Application.Behaviers;
using ToDoSystem.Application.Services.Auth;
using ToDoSystem.Application.Services.ToDo;

namespace ToDoSystem.Application
{
    public static class ApplicationDependencies
    {
        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddTransient<ITodoService, TodoService>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavier<,>));
            return services;
        }
    }
}
