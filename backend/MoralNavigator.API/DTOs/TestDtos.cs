// File: backend/MoralNavigator.API/DTOs/TestDto.cs

using System.Collections.Generic;

namespace MoralNavigator.API.DTOs
{
    public class TestDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public List<QuestionDto> Questions { get; set; } = new();

        public class QuestionDto
        {
            public int Id { get; set; }
            public string Text { get; set; } = null!;
            public string[] Options { get; set; } = System.Array.Empty<string>();
        }
    }
}
