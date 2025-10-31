using Microsoft.AspNetCore.Mvc;
using PhotoApp.Models;
using Microsoft.EntityFrameworkCore;
using PhotoApp.Services;

namespace PhotoApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request.Login, request.Password);
            if (result)
            {
                return Ok(new
                {
                    success = true,
                    user = _authService.CurrentUser,
                    role = _authService.CurrentUser?.Role?.Name
                });
            }
            return Unauthorized(new { success = false, message = "Неверный логин или пароль" });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _authService.Logout();
            return Ok(new { success = true });
        }

        [HttpGet("current")]
        public IActionResult GetCurrentUser()
        {
            if (_authService.IsAuthenticated)
            {
                return Ok(new
                {
                    user = _authService.CurrentUser,
                    role = _authService.CurrentUser?.Role?.Name
                });
            }
            return Unauthorized();
        }
    }

    public class LoginRequest
    {
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}