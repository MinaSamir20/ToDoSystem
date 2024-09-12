using ToDoSystem.Application.DTOs.Authentication;

namespace ToDoSystem.Application.Services.Auth
{
    public interface IAuthService
    {
        Task<int> GetUserId(string username);
        Task<AuthModel> RegisterAsync(RegisterDto model);
        Task<AuthModel> LoginAsync(LoginDto loginDto);
        Task<UserDto> GetUserByIdAsync(int id);
        Task<AuthModel> RefreshTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
        Task<AuthModel> GetTokenAsync(LoginDto model);
    }
}
