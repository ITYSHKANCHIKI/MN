using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MoralNavigator.API.Domain.Entities;
using MoralNavigator.API.Domain.Interfaces;
using MoralNavigator.API.DTOs;

namespace MoralNavigator.API.Services
{
    public class AuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _cfg;

        public AuthService(IUnitOfWork uow, IConfiguration cfg)
        {
            _uow = uow;
            _cfg = cfg;
        }

        public async Task<AuthResultDto> Register(RegisterDto dto)
        {
            if (await _uow.Users.ExistsAsync(u => u.Username == dto.Username))
                return new() { Success = false, Error = "Username already taken" };

            using var hmac = new HMACSHA512();
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                PasswordSalt = hmac.Key
            };
            await _uow.Users.AddAsync(user);
            await _uow.CompleteAsync();

            var token = CreateToken(user);
            return new() { Success = true, Token = token };
        }

        public async Task<AuthResultDto> Login(LoginDto dto)
        {
            var user = await _uow.Users
                .SingleOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null)
                return new() { Success = false, Error = "Invalid credentials" };

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));
            if (!computed.SequenceEqual(user.PasswordHash))
                return new() { Success = false, Error = "Invalid credentials" };

            var token = CreateToken(user);
            return new() { Success = true, Token = token };
        }

        private string CreateToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };
            var token = new JwtSecurityToken(
                issuer: _cfg["Jwt:Issuer"],
                audience: _cfg["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
