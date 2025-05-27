using Microsoft.AspNetCore.Mvc;
using MoralNavigator.API.Services;
using System.Security.Claims;

namespace MoralNavigator.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryController : ControllerBase
    {
        private readonly HistoryService _hist;
        public HistoryController(HistoryService hist) => _hist = hist;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // вытаскиваем userId из JWT-клейма
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var history = await _hist.GetForCurrentUserAsync(userId);
            return Ok(history);
        }
    }
}
