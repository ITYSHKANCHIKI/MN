// File: backend/MoralNavigator.API/Infrastructure/Repositories/UserRepository.cs

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoralNavigator.API.Domain.Entities;
using MoralNavigator.API.Domain.Interfaces;
using MoralNavigator.API.Infrastructure.Data;

namespace MoralNavigator.API.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db) => _db = db;

        public async Task<bool> ExistsAsync(string username)
        {
            return await _db.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
        }

        public async Task AddAsync(User user)
        {
            await _db.Users.AddAsync(user);
        }
    }
}
