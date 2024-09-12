using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ToDoSystem.Application.DTOs.Authentication;
using ToDoSystem.Application.Helper;
using ToDoSystem.Domain.Entities.Identity;
using ToDoSystem.Infrastructure.Database;

namespace ToDoSystem.Application.Services.Auth
{
    public class AuthService(AppDbContext db, IMapper mapper, UserManager<User> userManager, IOptions<JWT> jwt) : IAuthService
    {
        private readonly AppDbContext _db = db;
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<User> _userManager = userManager;
        private readonly JWT _jwt = jwt.Value;

        public async Task<int> GetUserId(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return 0;
            return user.Id;
        }
        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _db.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            return new UserDto
            {
                Email = user!.Email,
                UserName = user.UserName,
            };
        }

        public async Task<AuthModel> LoginAsync(LoginDto loginDto)
        {
            AuthModel authModel = new();

            var user = await _userManager.FindByEmailAsync(loginDto.Email!);

            if (user is null || !await _userManager.CheckPasswordAsync(user, loginDto.Password!))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.Roles = [.. (await _userManager.GetRolesAsync(user))];

            if (user.RefreshTokens!.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens!.FirstOrDefault(t => t.IsActive);
                authModel.RefreshToken = activeRefreshToken!.Token;
                authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens!.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            return authModel;
        }

        public async Task<AuthModel> RegisterAsync(RegisterDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new() { Message = $"{model.Email} is already registered!" };
            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new() { Message = $"{model.Username} is already registered!" };

            var user = _mapper.Map<User>(model);

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                    errors += $"{error.Description},";
                return new() { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, "User");
            var jwtSecurityToken = await CreateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens!.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            var authModel = new AuthModel()
            {
                Email = user.Email,
                IsAuthenticated = true,
                Roles = ["User"],
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn
            };
            return authModel;
        }

        public async Task<AuthModel> RefreshTokenAsync(string token)
        {
            var authModel = new AuthModel();
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens!.Any(t => t.Token == token));
            if (user == null)
            {
                authModel.Message = "Invalid Token";
                return authModel;
            }

            var refreshToken = user.RefreshTokens!.Single(t => t.Token == token);
            if (!refreshToken.IsActive)
            {
                authModel.Message = "Inactive Token";
                return authModel;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens!.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.Roles = [.. await _userManager.GetRolesAsync(user)];
            authModel.RefreshToken = refreshToken.Token;
            authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;

            return authModel;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens!.Any(t => t.Token == token));
            if (user == null)
                return false;

            var refreshToken = user.RefreshTokens!.Single(t => t.Token == token);
            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return true;
        }

        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var roleClaims = userRoles.Select(r => new Claim("roles", r)).ToList();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim("uid", user.Id.ToString())
            }
            .Union(userClaims)
            .Union(roleClaims);

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key)), SecurityAlgorithms.HmacSha256);

            return new(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays!),
                signingCredentials: signingCredentials);
        }
        private static RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
#pragma warning disable SYSLIB0023 // Type or member is obsolete
            using RNGCryptoServiceProvider generator = new();
#pragma warning restore SYSLIB0023 // Type or member is obsolete
            generator.GetBytes(randomNumber);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow,
            };
        }

        public async Task<AuthModel> GetTokenAsync(LoginDto model)
        {
            var authModel = new AuthModel();
            var user = await _userManager.FindByEmailAsync(model.Email!);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password!))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }
            var jwtSecurityToken = await CreateJwtToken(user);
            var roleslist = await _userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.Roles = roleslist.ToList();

            if (user.RefreshTokens!.Any(t => t.IsActive))
            {
                var activeRefreshTokrn = user.RefreshTokens!.FirstOrDefault(t => t.IsActive);
                authModel.RefreshToken = activeRefreshTokrn!.Token;
                authModel.RefreshTokenExpiration = activeRefreshTokrn.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens!.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            return authModel;
        }
    }
}
