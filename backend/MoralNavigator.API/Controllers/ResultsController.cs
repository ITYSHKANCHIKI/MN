// File: backend/MoralNavigator.API/Controllers/ResultsController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoralNavigator.API.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MoralNavigator.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ResultsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ResultsController(AppDbContext db)
        {
            _db = db;
        }

        // GET api/results/{resultId}/details
        [HttpGet("{resultId}/details")]
        public async Task<IActionResult> GetDetails(int resultId)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);

            var result = await _db.Results
                .Include(r => r.Test)
                    .ThenInclude(t => t.Questions)
                .Include(r => r.UserAnswers)
                .FirstOrDefaultAsync(r => r.Id == resultId && r.UserId == userId);

            if (result == null)
                return NotFound();

            var details = new
            {
                resultId = result.Id,
                testTitle = result.Test != null ? result.Test.Title : null,
                takenAt = result.TakenAt,
                score = result.Score,
                answers = result.UserAnswers.Select(a => new
                {
                    questionId = a.QuestionId,
                    selectedOption = a.SelectedOption
                }).ToList(),
                questions = result.Test.Questions.Select(q => new
                {
                    id = q.Id,
                    text = q.Text,
                    options = q.Options
                }).ToList()
            };

            return Ok(details);
        }
    }
}
