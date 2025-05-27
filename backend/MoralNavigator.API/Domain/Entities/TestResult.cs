namespace MoralNavigator.API.Domain.Entities
{
    public class TestResult
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TestId { get; set; }
        public DateTime TakenAt { get; set; }
        public int Score { get; set; }
    }
}