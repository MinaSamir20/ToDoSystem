using AutoMapper;
using ToDoSystem.Application.DTOs.Authentication;
using ToDoSystem.Application.Features.Todos.Models;
using ToDoSystem.Application.Features.Todos.Responces;
using ToDoSystem.Domain.Entities;
using ToDoSystem.Domain.Entities.Identity;

namespace ToDoSystem.Application.Profiles
{
    public partial class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ToDo
            CreateMap<Todo, CreateTodo>().ReverseMap();
            CreateMap<UpdateTodo, Todo>().ReverseMap();
            CreateMap<Todo, DeleteTodo>().ReverseMap();
            CreateMap<Todo, GetTodoResponse>().ReverseMap();

            //User
            CreateMap<User, RegisterDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
