// File: backend/MoralNavigator.API/Services/HistoryService.cs

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using MoralNavigator.API.Domain.Interfaces;
using MoralNavigator.API.DTOs;

namespace MoralNavigator.API.Services
{
    public class HistoryService
    {
        private readonly IUnitOfWork _uow;

        public HistoryService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<HistoryItemDto>> GetForUserAsync(int userId)
        {
            var results = await _uow.Results.GetByUserIdAsync(userId);
            return results.Select(r => new HistoryItemDto
            {
                ResultId = r.Id,
                TestTitle = r.Test?.Title ?? "—",
                Score = r.Score,
                TakenAt = r.TakenAt,
                Answers = r.UserAnswers.ToDictionary(a => a.QuestionId, a => a.SelectedOption)
            });
        }
    }
}
