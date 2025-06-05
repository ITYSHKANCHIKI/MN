// File: backend/MoralNavigator.API/Infrastructure/Repositories/UnitOfWork.cs

using System.Threading.Tasks;
using MoralNavigator.API.Domain.Interfaces;
using MoralNavigator.API.Infrastructure.Data;

namespace MoralNavigator.API.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            Users = new UserRepository(_db);
            Tests = new TestRepository(_db);
            Results = new TestResultRepository(_db);
        }

        public IUserRepository Users { get; }
        public ITestRepository Tests { get; }
        public ITestResultRepository Results { get; }

        public AppDbContext GetDbContext() => _db;

        public async Task<int> CompleteAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
