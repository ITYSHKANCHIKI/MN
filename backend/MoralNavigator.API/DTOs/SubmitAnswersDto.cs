// File: backend/MoralNavigator.API/DTOs/SubmitAnswersDto.cs

using System.Collections.Generic;

namespace MoralNavigator.API.DTOs
{
    public class SubmitAnswersDto
    {
        /// <summary>
        /// Словарь: ключ = questionId, значение = выбранный индекс (0 или 1).
        /// </summary>
        public Dictionary<int, int> Answers { get; set; } = new();
    }
}
