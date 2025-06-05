using MoralNavigator.API.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoralNavigator.API.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ExistsAsync(string username);
        Task<User?> SingleOrDefaultAsync(string username);
        Task AddAsync(User user);
    }

    public interface ITestRepository
    {
        Task<IEnumerable<Test>> GetAllAsync();
        Task<Test?> GetByIdAsync(int id);
    }

    public interface ITestResultRepository
    {
        Task AddAsync(TestResult result);
        Task<IEnumerable<TestResult>> GetByUserIdAsync(int userId);
    }

    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        ITestRepository Tests { get; }
        ITestResultRepository Results { get; }

        Task<int> CompleteAsync();
        // Возвращает DbContext, когда потребуется выполнить «сырые» запросы:
        AppDbContext GetDbContext();
    }
}
