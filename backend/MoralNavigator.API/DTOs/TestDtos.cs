using System.Collections.Generic;

namespace MoralNavigator.API.DTOs
{
    public record QuestionDto(int Id, string Text, IEnumerable<string> Options);
    public record TestDto(int Id, string Title, IEnumerable<QuestionDto> Questions);
    public record ResultDto(int Score);
}
