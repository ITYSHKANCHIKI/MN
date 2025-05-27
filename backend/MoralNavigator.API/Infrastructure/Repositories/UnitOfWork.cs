using System.Threading.Tasks;
using MoralNavigator.API.Domain.Entities;
using MoralNavigator.API.Domain.Interfaces;
using MoralNavigator.API.Infrastructure.Data;

namespace MoralNavigator.API.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _ctx;
        public IRepository<User> Users { get; }
        public IRepository<Test> Tests { get; }
        public IRepository<TestResult> Results { get; }

        public UnitOfWork(AppDbContext ctx)
        {
            _ctx = ctx;
            Users = new Repository<User>(ctx);
            Tests = new Repository<Test>(ctx);
            Results = new Repository<TestResult>(ctx);
        }

        public async Task<int> CompleteAsync() 
            => await _ctx.SaveChangesAsync();
    }
}
