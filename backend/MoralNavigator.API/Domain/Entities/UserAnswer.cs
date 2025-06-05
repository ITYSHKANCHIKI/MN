// File: backend/MoralNavigator.API/Domain/Entities/UserAnswer.cs

namespace MoralNavigator.API.Domain.Entities
{
    public class UserAnswer
    {
        public int Id { get; set; }

        public int TestResultId { get; set; }
        public TestResult TestResult { get; set; } = null!;

        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        public int SelectedOption { get; set; }
    }
}
