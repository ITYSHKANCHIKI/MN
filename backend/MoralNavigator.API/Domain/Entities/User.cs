using System.Collections.Generic;

namespace MoralNavigator.API.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
        public ICollection<TestResult> Results { get; set; } = new List<TestResult>();
    }
}
