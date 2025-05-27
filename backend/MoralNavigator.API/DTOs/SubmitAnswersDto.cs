// SubmitAnswersDto.cs
using System.Collections.Generic;

namespace MoralNavigator.API.DTOs
{
    public record SubmitAnswersDto(
        int UserId,
        Dictionary<int, int> Answers
    );
}
