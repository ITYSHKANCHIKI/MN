// File: backend/MoralNavigator.API/Infrastructure/Repositories/TestResultRepository.cs

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoralNavigator.API.Domain.Entities;
using MoralNavigator.API.Domain.Interfaces;
using MoralNavigator.API.Infrastructure.Data;

namespace MoralNavigator.API.Infrastructure.Repositories
{
    public class TestResultRepository : ITestResultRepository
    {
        private readonly AppDbContext _db;
        public TestResultRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(TestResult result)
        {
            await _db.Results.AddAsync(result);
        }

        public async Task<TestResult?> GetByIdAsync(int id)
        {
            return await _db.Results
                .Include(r => r.Test)       // захватим навигацию на Test
                .SingleOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<TestResult>> GetByUserIdAsync(int userId)
        {
            return await _db.Results
                .Where(r => r.UserId == userId)
                .Include(r => r.Test)
                .Include(r => r.UserAnswers)
                .ToListAsync();
        }
    }
}
