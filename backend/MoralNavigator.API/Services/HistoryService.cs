using MoralNavigator.API.Domain.Interfaces;
using MoralNavigator.API.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoralNavigator.API.Services
{
    public class HistoryService
    {
        private readonly IUnitOfWork _uow;
        public HistoryService(IUnitOfWork uow) => _uow = uow;

        public async Task<IEnumerable<HistoryDto>> GetForCurrentUserAsync(int userId)
        {
            // получаем все результаты, фильтруем по userId и мапим в DTO
            var allResults = await _uow.Results.GetAllAsync();
            return allResults
                .Where(r => r.UserId == userId)
                .Select(r => new HistoryDto(r.TestId, r.TakenAt, r.Score));
        }
    }
}
