// File: backend/MoralNavigator.API/Controllers/AuthController.cs

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MoralNavigator.API.Services;
using MoralNavigator.API.DTOs;

namespace MoralNavigator.API.Controllers
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            var (success, error) = await _authService.RegisterAsync(dto.Username, dto.Password);

            if (!success)
                return BadRequest(error);

            return Ok("User registered.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var (success, token, error) = await _authService.LoginAsync(dto.Username, dto.Password);

            if (!success)
                return Unauthorized(error);

            return Ok(new { access_token = token });
        }
    }

    public class RegisterDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }

    public class LoginDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
