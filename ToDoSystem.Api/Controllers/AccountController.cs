using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoSystem.Application.DTOs.Authentication;
using ToDoSystem.Application.Services.Auth;

namespace ToDoSystem.Api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController(IAuthService auth) : ControllerBase
    {
        private readonly IAuthService _auth = auth;

        [HttpPost, Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _auth.RegisterAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            SetRefreshTokenInCookie(result.RefreshToken!, result.RefreshTokenExpiration);

            return Ok(result);
        }


        [HttpPost, Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _auth.LoginAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }
        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            CookieOptions cookieOptions = new()
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
        [Authorize]
        [HttpPost, Route("revokeToken")]
        public async Task<IActionResult> RevokeToken(RevokeToken revoke)
        {
            var token = revoke.Token ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(token)) return BadRequest("Token is Required");

            var result = await _auth.RevokeTokenAsync(token);

            if (!result) return BadRequest("Token is Invalid!");
            return Ok();
        }
    }
}
