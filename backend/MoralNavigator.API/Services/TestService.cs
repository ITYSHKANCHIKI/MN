// File: backend/MoralNavigator.API/Services/TestService.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoralNavigator.API.Domain.Entities;
using MoralNavigator.API.Domain.Interfaces;
using MoralNavigator.API.DTOs;

namespace MoralNavigator.API.Services
{
    public class TestService
    {
        private readonly IUnitOfWork _uow;

        public TestService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        /// <summary>
        /// Возвращает список всех тестов вместе с вопросами (для страницы /api/tests).
        /// </summary>
        public async Task<IEnumerable<TestDto>> GetAvailableAsync()
        {
            var tests = await _uow.Tests.GetAllAsync();
            return tests.Select(t => new TestDto
            {
                Id = t.Id,
                Title = t.Title,
                Questions = t.Questions.Select(q => new TestDto.QuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Options = q.Options
                }).ToList()
            });
        }

        /// <summary>
        /// Возвращает конкретный тест по ID (для страницы /api/tests/{id}).
        /// </summary>
        public async Task<TestDto> GetByIdAsync(int id)
        {
            var test = await _uow.Tests.GetByIdAsync(id);
            if (test == null)
                throw new KeyNotFoundException($"Test with id={id} not found.");

            return new TestDto
            {
                Id = test.Id,
                Title = test.Title,
                Questions = test.Questions.Select(q => new TestDto.QuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Options = q.Options
                }).ToList()
            };
        }

        /// <summary>
        /// Сохраняет результаты прохождения теста: создаёт TestResult, сохраняет ответы и возвращает ID + Score.
        /// </summary>
        public async Task<ResultWithIdDto> SubmitAsync(int testId, int userId, SubmitAnswersDto dto)
        {
            var test = await _uow.Tests.GetByIdAsync(testId);
            if (test == null)
                throw new KeyNotFoundException($"Test with id={testId} not found.");

            // Считаем score
            var score = test.Questions
                .Count(q => dto.Answers.TryGetValue(q.Id, out var ans) && ans == q.CorrectOption);

            var result = new TestResult
            {
                TestId = testId,
                UserId = userId,
                Score = score,
                TakenAt = DateTime.UtcNow
            };

            await _uow.Results.AddAsync(result);
            await _uow.CompleteAsync();

            // Сохраняем детальные ответы
            foreach (var kvp in dto.Answers)
            {
                var questionId = kvp.Key;
                var selectedOption = kvp.Value;
                _uow.GetDbContext().UserAnswers.Add(new UserAnswer
                {
                    TestResultId = result.Id,
                    QuestionId = questionId,
                    SelectedOption = selectedOption
                });
            }
            await _uow.CompleteAsync();

            return new ResultWithIdDto
            {
                ResultId = result.Id,
                Score = score
            };
        }
    }
}