// File: backend/MoralNavigator.API/DTOs/HistoryDtos.cs

using System;
using System.Collections.Generic;

namespace MoralNavigator.API.DTOs
{
    public record HistoryEntryDto(int ResultId, string TestTitle, int Score, DateTime TakenAt);

    public record HistoryDto(IEnumerable<HistoryEntryDto> Entries);
}
