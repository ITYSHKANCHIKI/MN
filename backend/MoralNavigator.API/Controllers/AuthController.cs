using Microsoft.AspNetCore.Mvc;
using MoralNavigator.API.DTOs;
using MoralNavigator.API.Services;

namespace MoralNavigator.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _svc;
        public AuthController(AuthService svc) => _svc = svc;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var res = await _svc.Register(dto);
            if (!res.Success) return BadRequest(res);
            return Ok(res);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var res = await _svc.Login(dto);
            if (!res.Success) return BadRequest(res);
            return Ok(res);
        }
    }
}
