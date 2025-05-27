using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MoralNavigator.API.DTOs;
using MoralNavigator.API.Services;
using System.Threading.Tasks;

namespace MoralNavigator.API.Controllers
{
    [ApiController]
    [EnableCors("AllowAll")]                // разрешаем всем фронтам обращаться сюда
    [Route("api/[controller]")]
    public class TestsController : ControllerBase
    {
        private readonly TestService _testService;

        public TestsController(TestService testService)
        {
            _testService = testService;
        }

        // ноябрь CORS-предзапрос (preflight)
        [HttpOptions]
        [HttpOptions("{id}")]
        public IActionResult Options() => Ok();

        // GET api/tests
        [HttpGet]
        public async Task<IActionResult> GetAvailable()
        {
            var list = await _testService.GetAvailable();
            return Ok(list);
        }

        // GET api/tests/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var test = await _testService.GetById(id);
            return Ok(test);
        }

        // POST api/tests/{id} — принимаем ответы
        [HttpPost("{id}")]
        public async Task<IActionResult> Submit(
            int id,
            [FromBody] SubmitAnswersDto dto)
        {
            // dto.UserId должно приходить из фронта
            var result = await _testService.Submit(id, dto);
            return Ok(result);
        }
    }
}
