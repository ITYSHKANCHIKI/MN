// File: backend/MoralNavigator.API/Domain/Interfaces/ITestResultRepository.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using MoralNavigator.API.Domain.Entities;

namespace MoralNavigator.API.Domain.Interfaces
{
    public interface ITestResultRepository
    {
        Task AddAsync(TestResult result);

        /// <summary>
        /// Получить один результат по его ID (редко нужен напрямую в контроллерах, но всё же).
        /// </summary>
        Task<TestResult?> GetByIdAsync(int id);

        /// <summary>
        /// Получить все результаты, связанные с конкретным пользователем.
        /// </summary>
        Task<IEnumerable<TestResult>> GetByUserIdAsync(int userId);
    }
}
