// File: backend/MoralNavigator.API/Domain/Entities/Test.cs

using System.Collections.Generic;

namespace MoralNavigator.API.Domain.Entities
{
    public class Test
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        // Навигация: один Test → много Question
        public List<Question> Questions { get; set; } = new();

        // Навигация: один Test → много TestResult
        public List<TestResult> TestResults { get; set; } = new();
    }
}
