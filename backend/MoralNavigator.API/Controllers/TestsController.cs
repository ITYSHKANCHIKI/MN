using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MoralNavigator.API.DTOs;
using MoralNavigator.API.Services;

namespace MoralNavigator.API.Controllers
{
    [ApiController]
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    public class TestsController : ControllerBase
    {
        private readonly TestService _testService;
        public TestsController(TestService testService) => _testService = testService;

        // Обработка CORS preflight-запросов
        [HttpOptions]
        [HttpOptions("{id}")]
        public IActionResult Options() => Ok();

        // GET api/tests
        [HttpGet]
        public async Task<IActionResult> GetAvailable()
            => Ok(await _testService.GetAvailable());

        // GET api/tests/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
            => Ok(await _testService.GetById(id));

        // POST api/tests/{id}
        [Authorize]
        [HttpPost("{id}")]
        public async Task<IActionResult> Submit(int id, [FromBody] SubmitAnswersDto dto)
            => Ok(await _testService.Submit(id, dto));
    }
}
