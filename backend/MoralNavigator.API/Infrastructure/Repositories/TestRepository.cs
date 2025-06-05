// File: backend/MoralNavigator.API/Infrastructure/Repositories/TestRepository.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoralNavigator.API.Domain.Entities;
using MoralNavigator.API.Domain.Interfaces;
using MoralNavigator.API.Infrastructure.Data;

namespace MoralNavigator.API.Infrastructure.Repositories
{
    public class TestRepository : ITestRepository
    {
        private readonly AppDbContext _db;
        public TestRepository(AppDbContext db) => _db = db;

        public async Task<IEnumerable<Test>> GetAllAsync()
            => await _db.Tests
                .Include(t => t.Questions)
                .ToListAsync();

        public async Task<Test?> GetByIdAsync(int id)
            => await _db.Tests
                .Include(t => t.Questions)
                .SingleOrDefaultAsync(t => t.Id == id);
    }
}
