// File: backend/MoralNavigator.API/Domain/Entities/Question.cs

using System.Collections.Generic;

namespace MoralNavigator.API.Domain.Entities
{
    public class Question
    {
        public int Id { get; set; }

        public string Text { get; set; } = null!;

        public string[] Options { get; set; } = null!;   // хранится через конвертер

        public int CorrectOption { get; set; }

        // Внешний ключ к Test
        public int TestId { get; set; }
        public Test Test { get; set; } = null!;

        // Навигационное свойство: один Question → много UserAnswer
        public List<UserAnswer> UserAnswers { get; set; } = new();
    }
}
