using AutoMapper;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Mappings
{
    public class SprintProfile : Profile
    {
        public SprintProfile()
        {
            CreateMap<Sprint, SprintDto>()
                .ForMember(dest => dest.Modules, opt => opt.MapFrom(src => src.Modules));

            CreateMap<CreateSprintDto, Sprint>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

            CreateMap<UpdateSprintDto, Sprint>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}

