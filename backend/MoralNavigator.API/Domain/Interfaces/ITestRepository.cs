// File: backend/MoralNavigator.API/Domain/Interfaces/ITestRepository.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using MoralNavigator.API.Domain.Entities;

namespace MoralNavigator.API.Domain.Interfaces
{
    public interface ITestRepository
    {
        Task<IEnumerable<Test>> GetAllAsync();
        Task<Test?> GetByIdAsync(int id);
    }
}
