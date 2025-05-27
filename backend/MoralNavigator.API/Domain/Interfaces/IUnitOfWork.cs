using System.Threading.Tasks;
using MoralNavigator.API.Domain.Entities;
using System.Collections.Generic;

namespace MoralNavigator.API.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<User> Users { get; }
        IRepository<Test> Tests { get; }
        IRepository<TestResult> Results { get; }
        Task<int> CompleteAsync();
    }
}
