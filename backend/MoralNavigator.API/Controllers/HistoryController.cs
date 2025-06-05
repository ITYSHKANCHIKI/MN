// File: backend/MoralNavigator.API/Controllers/HistoryController.cs

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoralNavigator.API.Services;

namespace MoralNavigator.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class HistoryController : ControllerBase
    {
        private readonly HistoryService _historyService;

        public HistoryController(HistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHistory()
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var history = await _historyService.GetForUserAsync(userId);
            return Ok(history);
        }
    }
}
