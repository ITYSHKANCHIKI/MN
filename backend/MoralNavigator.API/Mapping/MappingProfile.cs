using AutoMapper;
using MoralNavigator.API.Domain.Entities;
using MoralNavigator.API.DTOs;

namespace MoralNavigator.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Test, TestDto>();
            CreateMap<Question, QuestionDto>();
            CreateMap<TestResult, HistoryDto>();
        }
    }
}
