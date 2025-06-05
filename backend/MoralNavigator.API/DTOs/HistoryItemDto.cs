// File: backend/MoralNavigator.API/DTOs/HistoryItemDto.cs

using System;

namespace MoralNavigator.API.DTOs
{
    public class HistoryItemDto
    {
        public int ResultId { get; set; }
        public string TestTitle { get; set; } = null!;
        public int Score { get; set; }
        public DateTime TakenAt { get; set; }
    }
}
