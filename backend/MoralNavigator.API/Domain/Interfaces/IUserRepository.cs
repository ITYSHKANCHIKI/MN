// File: backend/MoralNavigator.API/Domain/Interfaces/IUserRepository.cs

using System.Threading.Tasks;
using MoralNavigator.API.Domain.Entities;

namespace MoralNavigator.API.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> ExistsAsync(string username);
        Task AddAsync(User user);
    }
}
