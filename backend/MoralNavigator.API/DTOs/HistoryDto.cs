using System;

namespace MoralNavigator.API.DTOs
{
    public record HistoryDto(int TestId, DateTime TakenAt, int Score);
}