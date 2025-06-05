// File: backend/MoralNavigator.API/Mapping/MappingProfile.cs

using AutoMapper;
using MoralNavigator.API.Domain.Entities;
using MoralNavigator.API.DTOs;

namespace MoralNavigator.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Question → QuestionDto (Options мапятся напрямую)
            CreateMap<Question, TestDto.QuestionDto>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));

            // Test → TestDto
            CreateMap<Test, TestDto>();

            // TestResult → ResultWithIdDto
            CreateMap<TestResult, ResultWithIdDto>()
                .ForMember(dest => dest.ResultId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score));
        }
    }
}
