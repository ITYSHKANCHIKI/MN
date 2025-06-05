// File: backend/MoralNavigator.API/Controllers/ChatController.cs

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoralNavigator.API.Infrastructure.Data;
using MoralNavigator.API.Services;

namespace MoralNavigator.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatGptService _chatGpt;
        private readonly AppDbContext _db;

        public ChatController(IChatGptService chatGpt, AppDbContext db)
        {
            _chatGpt = chatGpt;
            _db = db;
        }

        [HttpPost("initiate")]
        public async Task<IActionResult> InitiateChat([FromBody] InitiateChatDto dto)
        {
            var userIdFromToken = int.Parse(User.FindFirst("id")!.Value);
            if (dto.UserId != userIdFromToken)
                return Forbid();

#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
            var result = await _db.Results
                .Include(r => r.Test)
                    .ThenInclude(t => t.Questions)
                .Include(r => r.UserAnswers)
                .FirstOrDefaultAsync(r => r.Id == dto.ResultId);
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.

            if (result == null)
                return NotFound();

            var systemPrompt = @"Вы — высококвалифицированный психолог, специализирующийся на моральных дилеммах и этическом выборе. 
Ваша задача — интерпретировать ответы пользователя, дать сочувственную обратную связь 
и задавать направляющие вопросы о моральных убеждениях пользователя. Отвечайте строго на русском языке, 
говорите спокойно и поддерживающе, избегайте резких суждений.";

            var userContent = new System.Text.StringBuilder();
            userContent.AppendLine($"Название теста: {result.Test?.Title}");
            userContent.AppendLine($"Баллы: {result.Score}");
            userContent.AppendLine($"Дата прохождения: {result.TakenAt:yyyy-MM-dd HH:mm}");
            userContent.AppendLine();
            userContent.AppendLine("Вопросы и ваши ответы:");
            foreach (var q in result.Test?.Questions ?? Enumerable.Empty<Domain.Entities.Question>())
            {
                var ua = result.UserAnswers.FirstOrDefault(a => a.QuestionId == q.Id);
                var selOpt = ua?.SelectedOption ?? -1;
                userContent.AppendLine($"Вопрос: {q.Text}");
                userContent.AppendLine($"Варианты: 0) {q.Options[0]}   1) {q.Options[1]}");
                if (selOpt >= 0 && selOpt < q.Options.Length)
                    userContent.AppendLine($"Ваш выбор: {selOpt} → {q.Options[selOpt]}");
                else
                    userContent.AppendLine("Ваш выбор: не указан");
                userContent.AppendLine();
            }

            var messages = new List<ChatMessageDto>
            {
                new ChatMessageDto("system", systemPrompt),
                new ChatMessageDto("user", userContent.ToString())
            };

            var botReply = await _chatGpt.SendChatAsync(messages);
            return Ok(new { botMessage = botReply });
        }

        [HttpPost("continue")]
        public async Task<IActionResult> ContinueChat([FromBody] ContinueChatDto dto)
        {
            var userIdFromToken = int.Parse(User.FindFirst("id")!.Value);
            if (dto.UserId != userIdFromToken)
                return Forbid();

            var result = await _db.Results.FindAsync(dto.ResultId);
            if (result == null)
                return NotFound();

            var systemPrompt = @"Вы — высококвалифицированный психолог, специализирующийся на моральных дилеммах и этическом выборе.
Продолжайте беседу, отвечайте строго на русском языке.";

            var messages = new List<ChatMessageDto> { new ChatMessageDto("system", systemPrompt) };
            foreach (var m in dto.Messages)
            {
                messages.Add(new ChatMessageDto(m.Role, m.Content));
            }

            var botReply = await _chatGpt.SendChatAsync(messages);
            return Ok(new { botMessage = botReply });
        }
    }

    public record InitiateChatDto(int ResultId, int UserId);

    public record ContinueChatDto(int ResultId, int UserId, List<ChatEntry> Messages);

    public record ChatEntry(string Role, string Content);
}
