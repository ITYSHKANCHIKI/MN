// File: backend/MoralNavigator.API/Domain/Entities/TestResult.cs

using System;
using System.Collections.Generic;

namespace MoralNavigator.API.Domain.Entities
{
    public class TestResult
    {
        public int Id { get; set; }

        // Связь с пользователем (теперь есть навигационное свойство)
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Связь с тестом
        public int TestId { get; set; }
        public Test Test { get; set; } = null!;

        // Дата прохождения
        public DateTime TakenAt { get; set; }

        // Набранные баллы
        public int Score { get; set; }

        // Список ответов пользователя на вопросы этого теста
        public List<UserAnswer> UserAnswers { get; set; } = new();
    }
}
