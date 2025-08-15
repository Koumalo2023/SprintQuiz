using AutoMapper;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Mappings
{
    public class CoursProfile : Profile
    {
        public CoursProfile()
        {
            CreateMap<Cours, CoursDto>()
                .ForMember(dest => dest.ModuleNom, opt => opt.MapFrom(src => src.Module.Nom));

            CreateMap<CreateCoursDto, Cours>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

            CreateMap<UpdateCoursDto, Cours>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}

