// File: backend/MoralNavigator.API/Controllers/TestsController.cs

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoralNavigator.API.DTOs;
using MoralNavigator.API.Services;

namespace MoralNavigator.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestsController : ControllerBase
    {
        private readonly TestService _testService;

        public TestsController(TestService testService)
        {
            _testService = testService;
        }

        // GET api/tests
        [HttpGet]
        public async Task<IActionResult> GetAvailable()
        {
            var list = await _testService.GetAvailableAsync();
            return Ok(list);
        }

        // GET api/tests/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var test = await _testService.GetByIdAsync(id);
            return Ok(test);
        }

        // POST api/tests/{id}
        [Authorize]
        [HttpPost("{id}")]
        public async Task<IActionResult> SubmitAnswers(int id, [FromBody] SubmitAnswersDto dto)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var result = await _testService.SubmitAsync(id, userId, dto);
            return Ok(result); // возвращаем { resultId, score }
        }
    }
}
