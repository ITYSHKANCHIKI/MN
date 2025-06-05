// File: backend/MoralNavigator.API/Domain/Interfaces/IUnitOfWork.cs

using System.Threading.Tasks;
using MoralNavigator.API.Domain.Entities;
using MoralNavigator.API.Infrastructure.Data;

namespace MoralNavigator.API.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        ITestRepository Tests { get; }
        ITestResultRepository Results { get; }
        Task<int> CompleteAsync();

        /// <summary>
        /// Дает доступ к самому DbContext
        /// </summary>
        AppDbContext GetDbContext();
    }
}
