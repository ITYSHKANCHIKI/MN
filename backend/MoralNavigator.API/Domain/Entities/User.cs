// File: backend/MoralNavigator.API/Domain/Entities/User.cs

using System.Collections.Generic;

namespace MoralNavigator.API.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;

        // Храним строковый хэш (BCrypt.HashPassword)
        public string PasswordHash { get; set; } = null!;

        // Необязательное поле соли (не обязательно использовать, BCrypt хранит соль в хэше)
        public string? PasswordSalt { get; set; }

        // Навигационное свойство: один User → много TestResult
        public ICollection<TestResult> Results { get; set; } = new List<TestResult>();
    }
}
