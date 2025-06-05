// File: backend/MoralNavigator.API/Services/AuthService.cs

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MoralNavigator.API.Domain.Entities;
using MoralNavigator.API.Domain.Interfaces;

namespace MoralNavigator.API.Services
{
    public class AuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _config;

        public AuthService(IUnitOfWork uow, IConfiguration config)
        {
            _uow = uow;
            _config = config;
        }

        public async Task<(bool Success, string? Error)> RegisterAsync(string username, string password)
        {
            if (await _uow.Users.ExistsAsync(username))
                return (false, "Username already taken.");

            // Хешируем через BCrypt (получаем string)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Username     = username,
                PasswordHash = passwordHash
                // PasswordSalt – не указываем, т.к. BCrypt хранит соль внутри hash
            };

            await _uow.Users.AddAsync(user);
            await _uow.CompleteAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Token, string? Error)> LoginAsync(string username, string password)
        {
            var user = await _uow.Users.GetByUsernameAsync(username);
            if (user == null)
                return (false, null, "Invalid credentials.");

            // Проверяем через BCrypt.Verify(string plain, string hashed)
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return (false, null, "Invalid credentials.");

            // Формируем JWT
            var jwtKey      = _config["Jwt:Key"]!;
            var jwtIssuer   = _config["Jwt:Issuer"]!;
            var jwtAudience = _config["Jwt:Audience"]!;

            var claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, username)
            };
            var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer:            jwtIssuer,
                audience:          jwtAudience,
                claims:            claims,
                expires:           DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return (true, tokenString, null);
        }
    }
}
