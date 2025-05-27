using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoralNavigator.API.Domain.Entities;
using MoralNavigator.API.Domain.Interfaces;
using MoralNavigator.API.DTOs;

namespace MoralNavigator.API.Services
{
    public class TestService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public TestService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TestDto>> GetAvailable()
        {
            var tests = await _uow.Tests.GetAllAsync(
                include: q => q.Include(t => t.Questions));
            return _mapper.Map<IEnumerable<TestDto>>(tests);
        }

        public async Task<TestDto> GetById(int id)
        {
            var test = await _uow.Tests.GetByIdAsync(
                id,
                include: q => q.Include(t => t.Questions));
            if (test == null) throw new KeyNotFoundException();
            return _mapper.Map<TestDto>(test);
        }

        public async Task<ResultDto> Submit(int id, SubmitAnswersDto dto)
        {
            var test = await _uow.Tests.GetByIdAsync(
                id,
                include: q => q.Include(t => t.Questions));
            if (test == null) throw new KeyNotFoundException();

            var score = test.Questions
                .Count(q => dto.Answers.TryGetValue(q.Id, out var ans) && ans == q.CorrectOption);

            var result = new TestResult
            {
                TestId = id,
                UserId = dto.UserId,
                Score = score,
                TakenAt = DateTime.UtcNow
            };
            await _uow.Results.AddAsync(result);
            await _uow.CompleteAsync();

            return new ResultDto(score);
        }
    }
}
